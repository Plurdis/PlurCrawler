﻿using System;
using System.Collections.Generic;
using System.Linq;

using PlurCrawler.Search.Base;
using PlurCrawler.Search;

using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;

namespace PlurCrawler.Search.Services.GoogleCSE
{
    public class GoogleCSESearcher : BaseSearcher<GoogleCSESearchOption, GoogleCSESearchResult>
    {
        /// <summary>
        /// <see cref="GoogleCSESearcher"/> 클래스를 초기화합니다.
        /// </summary>
        public GoogleCSESearcher()
        {
        }

        /// <summary>
        /// <see cref="GoogleCSESearcher"/> 클래스를 API Key와 검색 엔진 ID로 초기화합니다.
        /// </summary>
        /// <param name="apiKey">API Key 입니다.</param>
        /// <param name="searchEngineId">검색 엔진 ID입니다.</param>
        public GoogleCSESearcher(string apiKey, string searchEngineId)
        {
            Vertification(apiKey, searchEngineId);
        }
        
        /// <summary>
        /// Api Key 입니다.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// 검색 엔진 ID 입니다.
        /// </summary>
        public string SearchEngineId { get; private set; }

        /// <summary>
        /// ApiKey와 SearchEngineId를 새로 고칩니다. 유효성 검사는 하지 않습니다.
        /// </summary>
        /// <param name="apiKey">Api Key 입니다.</param>
        /// <param name="searchEngineId">검색 엔진 ID 입니다.</param>
        public void Vertification(string apiKey, string searchEngineId)
        {
            this.ApiKey = apiKey;
            this.SearchEngineId = searchEngineId;
                
            IsVerification = true;
        }

        /// <summary>
        /// Google Custom Search를 이용해 검색을 실시합니다.
        /// </summary>
        /// <param name="searchOption">Google Custom Search의 검색 옵션입니다.</param>
        /// <returns></returns>
        public override IEnumerable<GoogleCSESearchResult> Search(GoogleCSESearchOption searchOption)
        {
            CseResource.ListRequest request = BuildRequest(searchOption);

            IEnumerable<Result> results = Search(request, (long)searchOption.Offset, (long)searchOption.SearchCount);

            if (results.Count() == 0)
                return Enumerable.Empty<GoogleCSESearchResult>();

            return ConvertType(results);
        }

        #region [  Private Method  ]

        private CseResource.ListRequest BuildRequest(GoogleCSESearchOption option)
        {
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer
            {
                ApiKey = this.ApiKey
            });

            CseResource.ListRequest request = customSearchService.Cse.List(option.Query);

            request.Cx = SearchEngineId;
            request.Start = (long)option.Offset;

            // TODO : 날짜 설정 추가

            return request;
        }

        private IEnumerable<Result> Search(CseResource.ListRequest request, long offset, long targetCount)
        {
            var results = new List<Result>();
            IList<Result> paging = new List<Result>();
            int count = 0;

            while ((paging != null) && targetCount > 0)
            {
                request.Start = count * 10 + 1 + offset;
                if ((targetCount % 10) == 0)
                    request.Num = 10;
                else
                    request.Num = (targetCount % 10);

                paging = request.Execute().Items;

                if (paging != null)
                    results.AddRange(paging);

                count++;
                targetCount -= request.Num.Value;
            }

            return results;
        }

        private IEnumerable<GoogleCSESearchResult> ConvertType(IEnumerable<Result> resultList)
        {
            return resultList.Select(i => new GoogleCSESearchResult()
            {
                OriginalURL = i.Link,
                PublishedDate = null,
                Title = i.Title,
                Snippet = i.Snippet.Replace("\\n", Environment.NewLine)
            });
        }

        #endregion

        /// <summary>
        /// 검색 옵션을 무시한 채로 지정된 하루만 검색합니다.
        /// </summary>
        /// <param name="time">검색할 날짜입니다.</param>
        /// <param name="searchOption">검색 옵션입니다.</param>
        /// <returns></returns>
        public IEnumerable<GoogleCSESearchResult> SearchOneDay(DateTime time, GoogleCSESearchOption searchOption)
        {
            return null;
        }
    }
}
