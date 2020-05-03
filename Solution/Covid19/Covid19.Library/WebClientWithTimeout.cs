// <copyright file="WebClientWithTimeout.cs" company="julien_lefevre@outlook.fr">
//   Copyright (c) 2020 All Rights Reserved
//   <author>Julien LEFEVRE</author>
// </copyright>

using System;
using System.Net;

namespace Covid19.Library
{
    internal class WebClientWithTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest wr = base.GetWebRequest(address);
            wr.Timeout = 5000; // timeout in milliseconds (ms)
            return wr;
        }
    }
}
