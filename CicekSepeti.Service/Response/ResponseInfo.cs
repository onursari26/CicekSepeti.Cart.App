using CicekSepeti.Utility.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace CicekSepeti.Service.Response
{
    public class ResponseInfo<T>
    {
        public bool IsSuccessfull { get; private set; }

        [JsonIgnore]
        public HttpStatusCode HttpStatusCode { get; private set; }

        [JsonIgnore]
        public string ReturnUrl { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Count { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ErrorMessage { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Exception Exception { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T Data { get; private set; }

        private ResponseInfo()
        {

        }

        private static int SetCount(T data)
        {
            int count = 0;

            if (data != null)
                count = 1;

            ICollection col = data as ICollection;
            if (col != null)
                count = col.Count;

            return count;
        }

        public static ResponseInfo<T> Success(T data = default, HttpStatusCode httpStatusCode = HttpStatusCode.OK, string returnUrl = null)
        {
            return new ResponseInfo<T>
            {
                IsSuccessfull = true,
                Data = data,
                HttpStatusCode = httpStatusCode,
                ReturnUrl = returnUrl,
                Count = SetCount(data),
            };
        }

        public static ResponseInfo<T> Error(string errorMessage, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return new ResponseInfo<T>
            {
                IsSuccessfull = false,
                HttpStatusCode = httpStatusCode,
                ErrorMessage = errorMessage,
            };
        }

        public static ResponseInfo<T> Error(List<string> errorMessages, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            string errorMessage = string.Empty;
            foreach (var item in errorMessages)
            {
                errorMessage = !string.IsNullOrWhiteSpace(errorMessage) ? errorMessage += Environment.NewLine + item : errorMessage = item;
            }

            return new ResponseInfo<T>
            {
                IsSuccessfull = false,
                HttpStatusCode = httpStatusCode,
                ErrorMessage = errorMessage
            };
        }

        public static ResponseInfo<T> Fail(Exception exception, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            return new ResponseInfo<T>
            {
                IsSuccessfull = false,
                HttpStatusCode = httpStatusCode,
                Exception = exception,
                ErrorMessage = exception.GetInnerExceptionMessage()
            };
        }

        public static ResponseInfo<T> NotFound(HttpStatusCode httpStatusCode = HttpStatusCode.NotFound)
        {
            return new ResponseInfo<T>
            {
                IsSuccessfull = false,
                HttpStatusCode = httpStatusCode,
            };
        }
    }
}
