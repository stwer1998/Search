﻿using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Объект - контента Sitemap сайта
/// </summary>
namespace Search.IndexService
{
    internal class SiteMapContent
    {
        public Uri Url { get; set; }

        public List<string> Links { get; set; }

        //public List<string> Priority { get; set; }

        //public List<string> LastModified { get; set; }

        //public List<string> ChangeFrequency { get; set; }

    }

}
