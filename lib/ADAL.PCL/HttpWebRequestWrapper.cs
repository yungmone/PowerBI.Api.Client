//----------------------------------------------------------------------
// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
// Apache License 2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//----------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    internal class HttpWebRequestWrapper : IHttpWebRequest
    {
        private readonly HttpWebRequest request;

        private int timeoutInMilliSeconds = 30000;

        public HttpWebRequestWrapper(string uri)
        {
			this.request = (HttpWebRequest)WebRequest.Create(new Uri(uri));        
		}

        public RequestParameters BodyParameters { get; set; }

        public string Accept
        {
            set
            {
                this.request.Accept = value;
            }            
        }

        public string ContentType
        {
            set
            {
                this.request.ContentType = value;
            }
        }

        public string Method
        {
            set
            {
                this.request.Method = value;
            }
        }

        public bool UseDefaultCredentials
        {
            set
            {
                this.request.UseDefaultCredentials = value;
            }
        }

        public WebHeaderCollection Headers
        {
            get
            {
                return this.request.Headers;
            }
        }

        public int TimeoutInMilliSeconds
        {
            set
            {
                this.timeoutInMilliSeconds = value;
            }        
        }

        public async Task<IHttpWebResponse> GetResponseSyncOrAsync(CallState callState)
        {
            if (this.BodyParameters != null)
            {
                using (Stream stream = GetRequestStreamSyncOrAsync(callState))
                {
                    this.BodyParameters.WriteToStream(stream);
                }
            }

            return await PlatformPlugin.WebUIFactory.GetResponseWithTimeoutSyncOrAsync(this.request, this.timeoutInMilliSeconds, callState);
        }

		public Stream GetRequestStreamSyncOrAsync(CallState callState)
        {
			//return await this.request.GetRequestStreamAsync();
			var task = this.request.GetRequestStreamAsync();
			Task.WaitAll(task);
			return task.Result;
        }
    }
}