using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Meshmakers.Common.Shared.Serialization;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
// ReSharper disable once UnusedType.Global
public static class Serialization
{
    public static T? Deserialize<T>(string xmlString, string? rootElementName = null)
    {
        XmlRootAttribute? xmlRootAttribute = null;
        if (!string.IsNullOrWhiteSpace(rootElementName))
        {
            xmlRootAttribute = new XmlRootAttribute(rootElementName);
        }

        var serializer = new XmlSerializer(typeof(T), xmlRootAttribute);

        using var reader = new StringReader(xmlString);
        var deserialized = serializer.Deserialize(reader);
        if (deserialized == null)
        {
            return default;
        }

        return (T)deserialized;
    }

    public static string Serialize<T>(T o, string? rootElementName = null)
    {
        if (o == null)
        {
            throw new ArgumentNullException(nameof(o));
        }

        XmlRootAttribute? xmlRootAttribute = null;
        if (!string.IsNullOrWhiteSpace(rootElementName))
        {
            xmlRootAttribute = new XmlRootAttribute(rootElementName);
        }

        //Create our own namespaces for the output
        var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

        var settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true };

        var serializer = new XmlSerializer(typeof(T), xmlRootAttribute);

        using (var stream = new StringWriter())
        using (var writer = XmlWriter.Create(stream, settings))
        {
            serializer.Serialize(writer, o, emptyNamespaces);
            return stream.ToString();
        }
    }
}