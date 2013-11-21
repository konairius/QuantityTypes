namespace Units.Tests
{
    using System;
    using System.Globalization;
    using System.Threading;

    using NUnit.Framework;

    using ServiceStack.Text;

    [TestFixture]
    public class ServiceStackJsonTests
    {
        [Test]
        public void Serialize()
        {
            var obj = new TestObject { Distance = 1.23 * Length.Metre, Time = null };
            obj.Quantities.Add(10 * Temperature.DegreeCelsius);
            obj.Quantities.Add(25 * Mass.Kilogram);

            JsConfig.IncludeNullValues = true;
            // JsConfig<IQuantity>.SerializeFn = q => q.ToString(null, CultureInfo.InvariantCulture);
            var json = JsonSerializer.SerializeToString(obj);
            Assert.AreEqual("{\"Distance\":\"1.23 m\",\"Time\":null,\"Quantities\":[\"10 �C\",\"25 kg\"]}", json);
        }

        [Test]
        public void Serialize_NorwegianCulture()
        {
            var obj = new TestObject { Distance = 1.23 * Length.Metre, Time = null };

            Thread.CurrentThread.CurrentCulture = new CultureInfo("nb-NO");
            Assert.AreEqual("1,23 m", obj.Distance.ToString());

            JsConfig.IncludeNullValues = true;
            // JsConfig<Length>.SerializeFn = q => q.ToString(null, CultureInfo.InvariantCulture);
            var json = JsonSerializer.SerializeToString(obj);
            Assert.AreEqual("{\"Distance\":\"1.23 m\",\"Time\":null,\"Quantities\":[]}", json);
        }

        [Test]
        public void Deserialize_WithConverters()
        {
            var json = "{\"Distance\":\"1.23 m\",\"Time\":null}";

            JsConfig<Length>.DeSerializeFn = Length.ParseJson;
            JsConfig<Time>.DeSerializeFn = Time.ParseJson;

            var obj = JsonSerializer.DeserializeFromString<TestObject>(json);
            Assert.AreEqual(1.23 * Length.Metre, obj.Distance);
            Assert.AreEqual(null, obj.Time);
        }
    }
}