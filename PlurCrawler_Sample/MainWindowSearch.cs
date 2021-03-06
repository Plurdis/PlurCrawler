﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using PlurCrawler.Extension;
using PlurCrawler.Format;
using PlurCrawler.Format.Common;
using PlurCrawler.Search;
using PlurCrawler.Search.Base;
using PlurCrawler.Search.Common;
using PlurCrawler.Search.Services.GoogleCSE;
using PlurCrawler.Search.Services.Twitter;
using PlurCrawler.Search.Services.Youtube;
using PlurCrawler.Tokens.Credentials;

using PlurCrawler_Sample.Common;
using PlurCrawler_Sample.Controls;
using PlurCrawler_Sample.Export;
using PlurCrawler_Sample.Report;
using PlurCrawler_Sample.Report.Result;
using PlurCrawler_Sample.TaskLogs;

namespace PlurCrawler_Sample
{
    public partial class MainWindow
    {
        bool googleSearching = false,
            twitterSearching = false,
            youtubeSearching = false;

        public void AddLog(string message, TaskLogType type)
        {
            Dispatcher.Invoke(() =>
            {
                if (mainTabControl.SelectedIndex != 0)
                    mainTabControl.SelectedIndex = 0;
                TaskLogManager.AddLog(message, type);
                ScrollToEnd();
            });
        }

        #region [  Google CSE  ]

        public void SearchGoogle()
        {
            // 동일한 서비스는 끝나기 전까지 실행이 불가능함.
            if (googleSearching)
            {
                AddLog("이미 Google CSE 서비스에서 검색을 실행중입니다.", TaskLogType.Failed);
                return;
            }

            // 날짜 유효성 미리 검사

            if (SettingManager.GoogleCSESearchOption.UseDateSearch)
            {
                if (!SettingManager.GoogleCSESearchOption.DateRange.Vaild)
                {
                    AddLog("구글 검색의 날짜 설정이 잘못되었습니다.", TaskLogType.Failed);
                    return;
                }
            }

            googleSearching = true;
            _detailsOption.GoogleEnableChange(false);
            _vertManager.ChangeEditable(false, ServiceKind.GoogleCSE);
            lvGoogle.Items.Clear();

            Thread thr = new Thread(() =>
            {
                var sw = new Stopwatch();
                var googleCSESearcher = new GoogleCSESearcher();
                bool isCanceled = false;
                SearchResult info = SearchResult.Fail_APIError;
                GoogleCSESearchOption option = null;

                googleCSESearcher.SearchProgressChanged += Searcher_SearchProgressChanged;
                googleCSESearcher.SearchFinished += Searcher_SearchFinished;
                googleCSESearcher.ChangeInfoMessage += Searcher_ChangeInfoMessage;
                googleCSESearcher.SearchItemFound += Searcher_SearchItemFound;

                Dispatcher.Invoke(() =>
                {
                    sw.Start();
                    AddLog("Google CSE 검색 엔진을 초기화중입니다.", TaskLogType.SearchReady);

                    option = SettingManager.GoogleCSESearchOption;
                    option.Query = tbQuery.Text;

                    var tb = new TaskProgressBar();

                    tb.SetValue(title: "Google CSE 검색", message: "검색이 진행중입니다.", maximum: 1);

                    lvTask.Items.Insert(0, tb);
                    dict[googleCSESearcher] = tb;

                    if (option.OutputServices == OutputFormat.None)
                    {
                        tb.SetValue(message: "결과를 내보낼 위치가 없습니다.", maximum: 1);
                        AddLog("검색을 내보낼 위치가 없습니다.", TaskLogType.Failed);
                        tb.TaskFinished = true;
                        info = SearchResult.Fail_InvaildSetting;
                        isCanceled = true;
                    }

                    googleCSESearcher.Vertification(SettingManager.GoogleCredentials.Item1, SettingManager.GoogleCredentials.Item3);

                    if (!googleCSESearcher.IsVerification) // 인증되지 않았을 경우
                    {
                        tb.SetValue(message: "API키가 인증되지 않았습니다.", maximum: 1);
                        tb.TaskFinished = true;
                        AddLog("API키가 인증되지 않았습니다.", TaskLogType.Failed);

                        info = SearchResult.Fail_APIError;
                        isCanceled = true;
                    }
                });

                IEnumerable<GoogleCSESearchResult> googleResult = null;
                ExportResultPack pack = null;

                if (!isCanceled)
                {
                    try
                    {
                        googleResult = googleCSESearcher.Search(option);

                        if (googleResult.Count() == 0)
                        {
                            info = SearchResult.Fail_NoResult;
                            AddLog("검색 결과가 없습니다.", TaskLogType.Failed);
                        }
                        else
                        {
                            info = SearchResult.Success;
                            AddLog("검색 결과를 내보내는 중입니다.", TaskLogType.Searching);
                            pack = Export(option.OutputServices, googleResult, ServiceKind.GoogleCSE);
                        }

                    }
                    catch (InvaildOptionException)
                    {
                        AddLog("'Google CSE' 검색 중 오류가 발생했습니다. [날짜를 사용하지 않은 상태에서는 '하루 기준' 옵션을 사용할 수 없습니다.]", TaskLogType.Failed);
                        info = SearchResult.Fail_InvaildSetting;
                    }
                    catch (Exception)
                    {
                        AddLog("'Google CSE' 검색 중 오류가 발생했습니다. [잘못된 인증 정보를 입력했습니다.]", TaskLogType.Failed);
                        info = SearchResult.Fail_APIError;
                        Dispatcher.Invoke(() =>
                        {
                            SettingManager.GoogleCredentials.Item2 = VerifyType.Invalid;
                            SettingManager.GoogleCredentials.Item4 = VerifyType.Invalid;
                        });
                        
                    }
                    if (info != SearchResult.Success)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            dict[googleCSESearcher].SetValue(message: "설정 오류가 발생했습니다.", maximum: 1);
                            dict[googleCSESearcher].TaskFinished = true;
                        });
                    }
                    else if (info != SearchResult.Fail_NoResult)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            dict[googleCSESearcher].SetValue(message: "검색 결과가 없습니다.", maximum: 1);
                            dict[googleCSESearcher].TaskFinished = true;
                        });
                    }
                }

                Dispatcher.Invoke(() => {
                    AddLog($"Google CSE 검색에 소요된 시간 : {sw.ElapsedMilliseconds}ms", TaskLogType.Complete);

                    _taskReport.AddReport(new TaskReportData()
                    {
                        Query = option.Query,
                        RequestService = ServiceKind.GoogleCSE,
                        SearchCount = option.SearchCount,
                        SearchData = googleResult,
                        SearchDate = DateTime.Now,
                        SearchResult = info,
                        OutputFormat = option.OutputServices,
                        ExportResultPack = pack
                    });

                    _taskReport.SetLastReport();

                    googleSearching = false;
                    _detailsOption.GoogleEnableChange(true);
                    _vertManager.ChangeEditable(true, ServiceKind.GoogleCSE);
                });
            });

            thr.Start();
        }

        #endregion

        #region [  Twitter  ]

        public void TwitterSearch()
        {
            if (twitterSearching)
            {
                AddLog("이미 Twitter 서비스에서 검색을 실행중입니다.", TaskLogType.Failed);
                return;
            }

            if (!SettingManager.TwitterSearchOption.DateRange.Vaild)
            {
                AddLog("트위터 검색의 날짜 설정이 잘못되었습니다.", TaskLogType.Failed);
                return;
            }

            twitterSearching = true;
            _detailsOption.TwitterEnableChange(false);
            _vertManager.ChangeEditable(false, ServiceKind.Twitter);
            lvTwitter.Items.Clear();

            Thread thr = new Thread(() =>
            {
                var sw = new Stopwatch();
                var twitterSearcher = new TwitterSearcher();
                bool isCanceled = false;
                SearchResult info = SearchResult.Fail_APIError;
                TwitterSearchOption option = null;

                twitterSearcher.SearchProgressChanged += Searcher_SearchProgressChanged;
                twitterSearcher.SearchFinished += Searcher_SearchFinished;
                twitterSearcher.ChangeInfoMessage += Searcher_ChangeInfoMessage;
                twitterSearcher.SearchItemFound += Searcher_SearchItemFound;

                Dispatcher.Invoke(() =>
                {
                    sw.Start();

                    AddLog("Twitter 검색 엔진을 초기화중입니다.", TaskLogType.SearchReady);

                    option = SettingManager.TwitterSearchOption;
                    option.Query = tbQuery.Text;

                    var tb = new TaskProgressBar();

                    tb.SetValue(title: "Twitter 검색", message: "검색이 진행중입니다.", maximum: 1);

                    lvTask.Items.Insert(0, tb);
                    dict[twitterSearcher] = tb;

                    if (option.OutputServices == OutputFormat.None)
                    {
                        tb.SetValue(message: "결과를 내보낼 위치가 없습니다.", maximum: 1);
                        AddLog("검색을 내보낼 위치가 없습니다.", TaskLogType.Failed);

                        tb.TaskFinished = true;

                        info = SearchResult.Fail_InvaildSetting;
                        isCanceled = true;
                    }

                    if (!TwitterSearcher.IsVerification)
                    {
                        tb.SetValue(message: "API키가 인증되지 않았습니다.", maximum: 1);
                        AddLog("API키가 인증되지 않았습니다.", TaskLogType.Failed);

                        tb.TaskFinished = true;

                        info = SearchResult.Fail_APIError;
                        isCanceled = true;
                    }
                });

                IEnumerable<TwitterSearchResult> twitterResult = null;
                ExportResultPack pack = null;

                if (!isCanceled)
                {
                    try
                    {
                        twitterResult = twitterSearcher.Search(option);

                        if (twitterResult.Count() == 0)
                        {
                            info = SearchResult.Fail_NoResult;
                            AddLog("검색 결과가 없습니다.", TaskLogType.Failed);
                        }
                        else
                        {
                            info = SearchResult.Success;
                            AddLog("검색 결과를 내보내는 중입니다.", TaskLogType.Searching);
                            pack = Export(option.OutputServices, twitterResult, ServiceKind.Twitter);
                        }
                    }
                    catch (InvaildOptionException)
                    {
                        AddLog("'Twitter' 검색 중 오류가 발생했습니다. [날짜를 사용하지 않은 상태에서는 '하루 기준' 옵션을 사용할 수 없습니다.]", TaskLogType.Failed);
                        info = SearchResult.Fail_InvaildSetting;
                    }
                    catch (InternetUnstableException)
                    {
                        AddLog("'Twitter' 검색 중 오류가 발생했습니다. [인터넷 환경이 불안정한거 같습니다.]", TaskLogType.Failed);
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    AddLog($"Twitter 검색에 소요된 시간 : {sw.ElapsedMilliseconds}ms", TaskLogType.Complete);

                    _taskReport.AddReport(new TaskReportData()
                    {
                        Query = option.Query,
                        RequestService = ServiceKind.Twitter,
                        SearchCount = option.SearchCount,
                        SearchData = twitterResult,
                        SearchDate = DateTime.Now,
                        SearchResult = info,
                        OutputFormat = option.OutputServices,
                        ExportResultPack = pack
                    });

                    _taskReport.SetLastReport();

                    twitterSearching = false;
                    _detailsOption.TwitterEnableChange(true);
                    _vertManager.ChangeEditable(true, ServiceKind.Twitter);
                });
            });

            thr.Start();
        }

        #endregion

        #region [  Youtube  ]

        public void YoutubeSearch()
        {
            if (youtubeSearching)
            {
                AddLog("이미 Youtube 서비스에서 검색을 실행중입니다.", TaskLogType.Failed);
                return;
            }

            if (SettingManager.YoutubeSearchOption.UseDateSearch)
            {
                if (!SettingManager.YoutubeSearchOption.DateRange.Vaild)
                {
                    AddLog("유튜브 검색의 날짜 설정이 잘못되었습니다.", TaskLogType.Failed);
                    return;
                }
            }

            youtubeSearching = true;
            _detailsOption.YoutubeEnableChange(false);
            _vertManager.ChangeEditable(false, ServiceKind.Youtube);
            lvYoutube.Items.Clear();

            Thread thr = new Thread(() =>
            {
                var sw = new Stopwatch();
                var youtubeSearcher = new YoutubeSearcher();
                bool isCanceled = false;
                SearchResult info = SearchResult.Fail_APIError;
                YoutubeSearchOption option = YoutubeSearchOption.GetDefault();

                youtubeSearcher.SearchProgressChanged += Searcher_SearchProgressChanged;
                youtubeSearcher.SearchFinished += Searcher_SearchFinished;
                youtubeSearcher.ChangeInfoMessage += Searcher_ChangeInfoMessage;
                youtubeSearcher.SearchItemFound += Searcher_SearchItemFound;

                Dispatcher.Invoke(() =>
                {
                    sw.Start();

                    AddLog("Youtube 검색 엔진을 초기화중입니다.", TaskLogType.SearchReady);

                    option = SettingManager.YoutubeSearchOption;
                    option.Query = tbQuery.Text;

                    var tb = new TaskProgressBar();

                    tb.SetValue(title: "Youtube 검색", message: "검색이 진행중입니다.", maximum: 1);

                    lvTask.Items.Insert(0, tb);
                    dict[youtubeSearcher] = tb;

                    if (option.OutputServices == OutputFormat.None)
                    {
                        tb.SetValue(message: "결과를 내보낼 위치가 없습니다.", maximum: 1);
                        AddLog("검색을 내보낼 위치가 없습니다.", TaskLogType.Failed);

                        info = SearchResult.Fail_InvaildSetting;
                        isCanceled = true;
                    }

                    youtubeSearcher.Vertification(SettingManager.YoutubeCredentials.Item1);

                    if (!youtubeSearcher.IsVerification) // 인증되지 않았을 경우
                    {
                        tb.SetValue(message: "API키가 인증되지 않았습니다.", maximum: 1);
                        AddLog("API키가 인증되지 않았습니다.", TaskLogType.Failed);

                        info = SearchResult.Fail_APIError;
                        isCanceled = true;
                    }
                });

                IEnumerable<YoutubeSearchResult> youtubeResult = null;
                ExportResultPack pack = null;

                if (!isCanceled)
                {
                    try
                    {
                        youtubeResult = youtubeSearcher.Search(option);

                        if (youtubeResult.Count() == 0)
                        {
                            info = SearchResult.Fail_NoResult;
                            AddLog("검색 결과가 없습니다.", TaskLogType.Failed);
                        }
                        else
                        {
                            info = SearchResult.Success;
                            AddLog("검색 결과를 내보내는 중입니다.", TaskLogType.Searching);
                            pack = Export(option.OutputServices, youtubeResult, ServiceKind.Youtube);
                        }
                    }
                    catch (InvaildOptionException)
                    {
                        AddLog("'Youtube' 검색 중 오류가 발생했습니다. [날짜를 사용하지 않은 상태에서는 '하루 기준' 옵션을 사용할 수 없습니다.]", TaskLogType.Failed);
                        info = SearchResult.Fail_InvaildSetting;
                    }
                    catch (CredentialsTypeException ex)
                    {
                        // TODO: Exception에 오류 코드 추가
                        AddLog("'Youtube' 검색 중 오류가 발생했습니다. [Youtube의 API키가 올바르게 입력되지 않은거 같습니다.]", TaskLogType.Failed);

                        Dispatcher.Invoke(() => SettingManager.YoutubeCredentials.Item2 = VerifyType.Invalid);

                        info = SearchResult.Fail_APIError;
                    }
                }

                Dispatcher.Invoke(() => {

                    AddLog($"Youtube 검색에 소요된 시간 : {sw.ElapsedMilliseconds}ms", TaskLogType.Complete);

                    _taskReport.AddReport(new TaskReportData()
                    {
                        Query = option.Query,
                        RequestService = ServiceKind.Youtube,
                        SearchCount = option.SearchCount,
                        SearchData = youtubeResult,
                        SearchDate = DateTime.Now,
                        SearchResult = info,
                        OutputFormat = option.OutputServices,
                        ExportResultPack = pack
                    });

                    _taskReport.SetLastReport();

                    youtubeSearching = false;
                    _detailsOption.YoutubeEnableChange(true);
                    _vertManager.ChangeEditable(true, ServiceKind.Youtube);
                });
            });

            thr.Start();
        }

        #endregion

        #region [  Keyword Search  ]

        public void KeywordSearch(string keyword, bool moveExternalSite)
        {

        }

        #endregion

        #region [  Common Events  ]

        private void Searcher_SearchFinished(object sender, SearchFinishedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TaskProgressBar itm = dict[sender as ISearcher];
                itm.SetValue(value: itm.Maximum, message: "검색이 완료되었습니다.");

                itm.TaskFinished = true;

                TaskLogManager.AddLog("검색이 완료되었습니다.", TaskLogType.Searching);

                switch (e.ServiceKind)
                {
                    case ServiceKind.GoogleCSE:
                        SettingManager.GoogleCredentials.Item2 = VerifyType.Verified;
                        SettingManager.GoogleCredentials.Item4 = VerifyType.Verified;
                        break;
                    case ServiceKind.Youtube:
                        SettingManager.YoutubeCredentials.Item2 = VerifyType.Verified;
                        break;
                }
            });
        }

        private void Searcher_SearchProgressChanged(object sender, ProgressEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                var itm = dict[sender as ISearcher];
                itm.SetValue(maximum: args.Maximum, value: args.Value);
            });
        }

        private void Searcher_ChangeInfoMessage(object sender, MessageEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var itm = dict[sender as ISearcher];
                itm.SetValue(message: e.Message);
            });
        }

        private void Searcher_SearchItemFound(object sender, SearchResultEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (e.Kind)
                {
                    case ServiceKind.GoogleCSE:
                        lvGoogle.Items.Add((GoogleCSESearchResult)e.Result);
                        if (AutoPreviewItemFocus)
                            tcPreview.SelectedIndex = 0;
                        break;
                    case ServiceKind.Youtube:
                        lvYoutube.Items.Add((YoutubeSearchResult)e.Result);
                        if (AutoPreviewItemFocus)
                            tcPreview.SelectedIndex = 2;
                        break;
                    case ServiceKind.Twitter:
                        lvTwitter.Items.Add((TwitterSearchResult)e.Result);
                        if (AutoPreviewItemFocus)
                            tcPreview.SelectedIndex = 1;
                        break;
                }
            });
        }

        #endregion

        public string GetServiceString(ServiceKind serviceKind)
        {
            switch (serviceKind)
            {
                case ServiceKind.GoogleCSE:
                    return "_google";
                case ServiceKind.Youtube:
                    return "_youtube";
                case ServiceKind.Twitter:
                    return "_twitter";
            }

            return string.Empty;
        }

        public ExportResultPack Export(OutputFormat format, IEnumerable<ISearchResult> result, ServiceKind serviceKind)
        {
            ExportResultPack pack = new ExportResultPack();
            

            if (format.HasFlag(OutputFormat.Json))
            {
                string folder = SettingManager.ExportOptionSetting.JsonFolderLocation;
                string fileName = SettingManager.ExportOptionSetting.JsonFileName;

                if (fileName.IsNullOrEmpty())
                {
                    AddLog("파일을 Json 형식으로 내보내기 실패했습니다. [파일명 입력란이 비어있습니다.]", TaskLogType.Failed);
                    Dispatcher.Invoke(() => pack.JsonExportResult = JsonExportResult.Fail_FileNameNull);
                }
                else
                {
                    if (Directory.Exists(folder))
                    {
                        try
                        {
                            string fullPath = Path.Combine(folder, $"{fileName}{GetServiceString(serviceKind)}.json");

                            if (ExportManager.JsonExport(fullPath, result, SettingManager.ExportOptionSetting.JsonSort))
                            {
                                AddLog($"Json으로 성공적으로 내보냈습니다. 저장 위치 : {fullPath}", TaskLogType.Complete);
                                pack.JsonExportResult = JsonExportResult.Success;
                            }
                            else
                            {
                                AddLog($"Json으로 내보낼 파일 위치에 접근 실패했습니다. 위치 : {fullPath}", TaskLogType.Failed);
                                pack.JsonExportResult = JsonExportResult.Fail_FileAccessDenied;
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLog($"Json으로 내보내던 중 알 수 없는 오류가 발생했습니다.{Environment.NewLine}{Environment.NewLine}{ex.ToString()}", TaskLogType.System);
                            pack.JsonExportResult = JsonExportResult.Unknown;
                        }
                    }
                    else
                    {
                        if (folder.IsNullOrEmpty())
                        {
                            AddLog("파일을 Json 형식으로 내보내기 실패했습니다. [폴더 입력란이 비어있습니다.]", TaskLogType.Failed);
                            pack.JsonExportResult = JsonExportResult.Fail_FileDirectoryNull;
                        }
                        else if (!Directory.Exists(folder))
                        {
                            AddLog("파일을 Json 형식으로 내보내기 실패했습니다. [해당하는 경로가 없습니다.]", TaskLogType.Failed);
                            pack.JsonExportResult = JsonExportResult.Fail_FileDirectoryNotExists;
                        }
                    }
                }
            }
            if (format.HasFlag(OutputFormat.CSV))
            {
                string folder = SettingManager.ExportOptionSetting.CSVFolderLocation;
                string fileName = SettingManager.ExportOptionSetting.CSVFileName;

                if (fileName.IsNullOrEmpty())
                {
                    AddLog("파일을 CSV 형식으로 내보내기 실패했습니다. [파일명 입력란이 비어있습니다.]", TaskLogType.Failed);
                    pack.CSVExportResult = CSVExportResult.Fail_FileNameNull;
                }
                else
                {
                    if (Directory.Exists(folder))
                    {
                        try
                        {
                            string fullPath = Path.Combine(folder, $"{fileName}{GetServiceString(serviceKind)}.csv");

                            if (ExportManager.CSVExport(fullPath, result))
                            {
                                AddLog($"CSV로 성공적으로 내보냈습니다. 저장 위치 : {fullPath}", TaskLogType.Complete);
                                pack.CSVExportResult = CSVExportResult.Success;
                            }
                            else
                            {
                                AddLog($"CSV로 내보낼 파일 위치에 접근 실패했습니다. 위치 : {fullPath}", TaskLogType.Failed);
                                pack.CSVExportResult = CSVExportResult.Fail_FileAccessDenied;
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLog($"CSV로 내보내던 중 알 수 없는 오류가 발생했습니다.{Environment.NewLine}{Environment.NewLine}{ex.ToString()}", TaskLogType.System);
                            pack.CSVExportResult = CSVExportResult.Unknown;
                        }
                    }
                    else
                    {
                        if (folder.IsNullOrEmpty())
                        {
                            AddLog("파일을 CSV 형식으로 내보내기 실패했습니다. [폴더 입력란이 비어있습니다.]", TaskLogType.Failed);
                            pack.CSVExportResult = CSVExportResult.Fail_FileDirectoryNull;
                        }
                        else if (!Directory.Exists(folder))
                        {
                            AddLog("파일을 CSV 형식으로 내보내기 실패했습니다. [해당하는 경로가 없습니다.]", TaskLogType.Failed);
                            pack.CSVExportResult = CSVExportResult.Fail_FileDirectoryNotExists;
                        }
                    }
                }
            }
            if (format.HasFlag(OutputFormat.MySQL))
            {
                pack.MySQLExportResult = MySQLExportResult.NotSet;

                MySQLFormat<ISearchResult> mySQLFormat = null;
                try
                {
                    if (SettingManager.ExportOptionSetting.MySQLManualInput)
                    {
                        if (SettingManager.ExportOptionSetting.MySQLConnString.IsNullOrEmpty())
                        {
                            AddLog("파일을 MySQL로 내보내기 실패했습니다. [데이터베이스 연결문이 입력되지 않았습니다.]", TaskLogType.Failed);
                            pack.MySQLExportResult = MySQLExportResult.Fail_NotEnoughConnectData;
                        }

                        if (pack.MySQLExportResult == MySQLExportResult.NotSet)
                            mySQLFormat = new MySQLFormat<ISearchResult>(SettingManager.ExportOptionSetting.MySQLConnString);
                    }
                    else
                    {
                        if (SettingManager.ExportOptionSetting.MySQLConnAddr.IsNullOrEmpty())
                        {
                            AddLog("파일을 MySQL로 내보내기 실패했습니다. [연결 주소가 입력되지 않았습니다.]", TaskLogType.Failed);
                            pack.MySQLExportResult = MySQLExportResult.Fail_NotEnoughConnectData;
                        }
                        else if (SettingManager.ExportOptionSetting.MySQLDatabaseName.IsNullOrEmpty())
                        {
                            AddLog("파일을 MySQL로 내보내기 실패했습니다. [데이터베이스 이름이 입력되지 않았습니다.]", TaskLogType.Failed);
                            pack.MySQLExportResult = MySQLExportResult.Fail_NotEnoughConnectData;
                        }
                        else if (SettingManager.ExportOptionSetting.MySQLUserID.IsNullOrEmpty())
                        {
                            AddLog("파일을 MySQL로 내보내기 실패했습니다. [데이터베이스 사용자 이름이 입력되지 않았습니다.]", TaskLogType.Failed);
                            pack.MySQLExportResult = MySQLExportResult.Fail_NotEnoughConnectData;
                        }
                        else if (SettingManager.ExportOptionSetting.MySQLUserPassword.IsNullOrEmpty())
                        {
                            AddLog("파일을 MySQL로 내보내기 실패했습니다. [데이터베이스 사용자 비밀번호가 입력되지 않았습니다.]", TaskLogType.Failed);
                            pack.MySQLExportResult = MySQLExportResult.Fail_NotEnoughConnectData;
                        }

                        if (pack.MySQLExportResult == MySQLExportResult.NotSet)
                            mySQLFormat = new MySQLFormat<ISearchResult>(SettingManager.ExportOptionSetting.MySQLConnAddr,
                                SettingManager.ExportOptionSetting.MySQLUserID,
                                SettingManager.ExportOptionSetting.MySQLUserPassword,
                                SettingManager.ExportOptionSetting.MySQLDatabaseName);
                    }
                }
                catch (Exception)
                {
                }

                if (mySQLFormat == null)
                {
                    AddLog("파일을 MySQL로 내보내기 실패했습니다. [데이터베이스 연결 정보중 하나가 잘못되었습니다.]", TaskLogType.Failed);
                }
                else
                {
                    try
                    {
                        ExportManager.MySQLExport(result, mySQLFormat);
                        AddLog("MySQL로 성공적으로 내보냈습니다.", TaskLogType.Complete);
                        pack.MySQLExportResult = MySQLExportResult.Success;
                    }
                    catch (Exception ex)
                    {
                        AddLog("파일을 MySQL로 내보내기 실패했습니다. [알 수 없는 오류가 발생했습니다.]", TaskLogType.Failed);
                        AddLog(ex.ToString(), TaskLogType.Failed);
                        pack.MySQLExportResult = MySQLExportResult.Fail_UnkownError;
                    }
                }
            }
            if (format.HasFlag(OutputFormat.AccessDB))
            {
                string folder = SettingManager.ExportOptionSetting.AccessFolderLocation;
                string fileName = SettingManager.ExportOptionSetting.AccessFileName;

                if (fileName.IsNullOrEmpty())
                {
                    AddLog("파일을 Access DB 형식으로 내보내기 실패했습니다. [파일명 입력란이 비어있습니다.]", TaskLogType.Failed);
                    pack.AccessExportResult = AccessExportResult.Fail_FileNameNull;
                }
                else
                {
                    if (Directory.Exists(folder))
                    {
                        try
                        {
                            string fullPath = Path.Combine(folder, $"{fileName}.accdb");
                            if (ExportManager.AccessDBExport(fullPath, result))
                            {
                                AddLog($"Access DB로 성공적으로 내보냈습니다. 저장 위치 : {fullPath}", TaskLogType.Complete);
                                pack.AccessExportResult = AccessExportResult.Success;
                            }
                            else
                            {
                                AddLog($"Access DB로 내보낼 파일 위치에 접근 실패했습니다. 위치 : {fullPath}", TaskLogType.Failed);
                                pack.AccessExportResult = AccessExportResult.Fail_FileAccessDenied;
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLog($"파일을 Access DB로 내보내기 실패했습니다. [알 수 없는 오류가 발생했습니다.]", TaskLogType.Failed);
                            AddLog(ex.ToString(), TaskLogType.Failed);

                            pack.AccessExportResult = AccessExportResult.Unknown;
                        }
                    }
                    else
                    {
                        if (folder.IsNullOrEmpty())
                        {
                            AddLog("파일을 Access DB 형식으로 내보내기 실패했습니다. [폴더 입력란이 비어있습니다.]", TaskLogType.Failed);
                            pack.AccessExportResult = AccessExportResult.Fail_FileDirectoryNull;
                        }
                        else if (!Directory.Exists(folder))
                        {
                            AddLog("파일을 Access DB 형식으로 내보내기 실패했습니다. [해당하는 경로가 없습니다.]", TaskLogType.Failed);
                            pack.AccessExportResult = AccessExportResult.Fail_FileDirectoryNotExists;
                        }
                    }
                }
            }

            return pack;
        }
    }
}
