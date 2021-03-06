﻿using System;

namespace PlurCrawler.Search.Base
{
    /// <summary>
    /// 검색 결과의 기본 인터페이스입니다.
    /// </summary>
    public interface ISearchResult
    {
        /// <summary>
        /// 해당 검색 결과가 게시된 날짜를 가져옵니다.
        /// </summary>
        DateTime? PublishedDate { get; }
        
        /// <summary>
        /// 기본 URL을 가져옵니다.
        /// </summary>
        string OriginalURL { get; }

        /// <summary>
        /// 제목을 나타냅니다.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 검색을 위해 입력했던 키워드를 나타냅니다.
        /// </summary>
        string Keyword { get; set; }
    }
}
