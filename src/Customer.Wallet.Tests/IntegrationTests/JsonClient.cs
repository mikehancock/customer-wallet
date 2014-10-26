namespace Customer.Wallet.UnitTests.IntegrationTests
{
    using System;
    using System.Web;

    using RestSharp;

    public class JsonClient
    {
        private readonly RestClient restClient;

        public JsonClient()
            : this(new RestClient())
        {
        }

        public JsonClient(RestClient restClient)
        {
            this.restClient = restClient;
        }

        public virtual IRestResponse Get(IRestRequest request)
        {
            request.Method = Method.GET;
            return this.ExecuteRequest(request);
        }

        public virtual IRestResponse Get(Uri resourceUri)
        {
            var request = new JsonRequestBuilder().WithUri(resourceUri);
            request.Method = Method.GET;
            return this.ExecuteRequest(request);
        }

        public virtual T Get<T>(IRestRequest request) where T : new()
        {
            request.Method = Method.GET;
            return this.ExecuteRequest<T>(request);
        }

        public virtual IRestResponse Post(IRestRequest request)
        {
            request.Method = Method.POST;
            return this.ExecuteRequest(request);
        }

        public virtual T Post<T>(IRestRequest request) where T : new()
        {
            request.Method = Method.POST;
            return this.ExecuteRequest<T>(request);
        }

        public virtual IRestResponse Put(IRestRequest request)
        {
            request.Method = Method.PUT;
            return this.ExecuteRequest(request);
        }

        public virtual IRestResponse Patch(IRestRequest request)
        {
            request.Method = Method.PATCH;
            return this.ExecuteRequest(request);
        }

        public virtual IRestResponse Delete(IRestRequest request)
        {
            request.Method = Method.DELETE;
            return this.ExecuteRequest(request);
        }

        private static void RethrowResponseExceptions(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                var message = string.Format(
                    "Response has invalid status code '{0}', with response status '{1}' for url '{2}'. Error message: '{3}'",
                    response.StatusCode,
                    response.ResponseStatus,
                    response.ResponseUri,
                    response.ErrorMessage);
                throw new HttpException((int)response.StatusCode, message);
            }
        }

        private IRestResponse ExecuteRequest(IRestRequest request)
        {
            var response = this.restClient.Execute(request);
            RethrowResponseExceptions(response);
            return response;
        }

        private T ExecuteRequest<T>(IRestRequest request) where T : new()
        {
            var response = this.restClient.Execute<T>(request);
            RethrowResponseExceptions(response);
            return response.Data;
        }
    }
}
