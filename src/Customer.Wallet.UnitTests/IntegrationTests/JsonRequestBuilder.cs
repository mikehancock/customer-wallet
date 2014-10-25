namespace Customer.Wallet.UnitTests.IntegrationTests
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using RestSharp;

    public class JsonRequestBuilder : RestRequest
    {
        public JsonRequestBuilder()
        {
            this.RequestFormat = DataFormat.Json;
        }

        public JsonRequestBuilder WithUri(Uri resourceUri)
        {
            this.Resource = resourceUri.AbsoluteUri;
            return this;
        }

        public JsonRequestBuilder ForHttpVerb(Method method)
        {
            this.Method = method;
            return this;
        }

        public JsonRequestBuilder WithBody<T>(T resource) where T : new()
        {
            this.AddParameter("text/json", JsonConvert.SerializeObject(resource), ParameterType.RequestBody);

            return this;
        }

        public JsonRequestBuilder WithHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                this.AddHeader(header.Key, header.Value);
            }

            return this;
        }

        public JsonRequestBuilder WithParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                this.AddParameter(parameter.Key, parameter.Value);
            }

            return this;
        }
    }
}
