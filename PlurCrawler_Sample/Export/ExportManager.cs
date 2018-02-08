﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlurCrawler.Format;
using PlurCrawler.Search.Base;

namespace PlurCrawler_Sample.Export
{
    static class ExportManager
    {
        public static void JsonExport<TResult>(string fileLocation, IEnumerable<TResult> searchResult) 
            where TResult : ISearchResult
        {
            var jsonFormat = new JsonFormat<TResult>();

            string str = jsonFormat.FormattingData(searchResult);

            jsonFormat.SaveFile(fileLocation, str);
        }
    }
}