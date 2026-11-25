---
description: Metrics collections are bounded by meter and checkpoint names, never by the number of runs.
applies_to: src/Metrics/**, test/MetricsTests/**
---

## Metrics: collections are bounded by names, never by runs

`Meshmakers.Common.Metrics` is called from hot paths: the engine wraps every runtime query in a `RuntimeMeter`
(`IMetricsContext.CreateRuntimeMeter`). No service starts the publisher (`MetricsContext.InitProvider`), so nothing
ever pops the collected data.

Rule: `RuntimeDataCollection` keeps one **aggregate** (`RuntimeResult`: calls, min, max, average per checkpoint) per
meter name, and never the individual `Checkpoint` objects of the runs. Memory is bounded by the number of meter and
checkpoint names, whatever the number of runs.

It used to append every checkpoint of every run to a `ConcurrentBag` until somebody popped it. A production mesh
adapter (about 4000 pipeline executions per hour) retained 1.6 GiB of managed heap in three days
(gen2 22 -> 1663 MiB, LOH 27 -> 429 MiB), found with `dotnet-gcdump`: `Checkpoint` objects and their bag arrays.

A new collection in a hot path must say how it is bounded, and needs a test that adds many runs,
tracks (e.g. with a `WeakReference`) **every** object the collection could retain from each run, not one sample, forces
a collection and asserts that none of them is still reachable. A single `WeakReference` can pass while another object
of every run stays referenced. See `RuntimeDataCollectionTests`.
