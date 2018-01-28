﻿using System;
using PlurCrawler.Search.Base;

namespace PlurCrawler.Search.Services.GoogleCSE
{
    public class GoogleCSESearchOption : IDateSearchOption
    {
        public string Query { get; set; }

        public ulong SearchCount { get; set; }

        public ulong Offset { get; set; }
        public DateTime? PublishedAfter { get; set; }
        public DateTime? PublishedBefore { get; set; }
        public LanguageCode Language { get; set; }
    }
}