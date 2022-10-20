# Common.Metrics

## Basic usage

Currently the only metrics supported are runtime data.<br>
A requirement is that `MetricsContext` has been made available via dependency injection.<br>
The usage is divided into __collect data__, __retrieve results__ or __publish data via provider service__.

### Collect data

The following example shows a runtime measurement of the method `Foo` with an optional checkpoint and optional explicit
stop.<br>
The measurement automatically uses the method name, in this case __Foo__.

```csharp
public void Foo()
{         
    // Initiate and start the runtime meter
    using var meter = metricsContext.CreateRuntimeMeter();
    
    DoFancyStuff();
    
    // Optionaly, add checkpoints to the measrement
    meter.SetCheckpoint("CustomCheckpoint");
        
    DoFancyStuffAgain();
    
    // Optionaly, end the measrement
    meter.Stop();
    // If it is not explicitly stopped, then it is automatically stopped when the method ends.
}
```

### Retrieve results

In this example, the results of the data collected above are retrieved.<br>
Two methods are provided for this purpose:

- `GetRuntimeResult(<MethodName>)` - Returns the current results without removing them.
- `PopRuntimeResult(<MethodName>)` - Returns and removes the current results.

```csharp
public void Bar()
{
    // The data is retrieved and can be further used afterwards.
    var result = metrics.PopRuntimeResult("Foo");
    
    // Get the average total runtime of the entire measurement
    _log.Debuag($"Runtime total {result.GetAverageTotalInMs()}");
    // Get the average total runtime up to checkpoint "CustomCheckpoint".
    _log.Debuag($"Runtime checkpoint {result.GetAverageTotalInMs('CustomCheckpoint')}");
}
```

### Publish data via provider service

This approach provides metrics data about the endpoint `http://loaclhost:9184/metrics` as YAML.<br>
The data displayed in the YAML is refreshed every 10 seconds. Both port and publish interval are configurable.

```csharp
public void Baz()
{
    // Initialises the metrics provider.
    // Optionaly, configure publish interval and port.
    metricsContext.InitProvider(intervalMs: 5000, port: 8989);
}
```


