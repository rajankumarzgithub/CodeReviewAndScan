using System;
using System.Collections.Generic;
using System.IO;
using CommonUtils = EbixExchange.LifeSpeed.CommonUtils;
using System.Data;

public partial class scripts_client_scripts : System.Web.UI.Page
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (this.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            this.Response.ContentType = "text/javascript";

            if (this.Request.UrlReferrer != null)
            {
                string pageCode = (this.Request.QueryString["code"] ?? string.Empty).Trim().ToLower();
                string appID = this.Request.QueryString["appID"];

                bool compressScripts = false;

                #compressScripts = CommonUtils.SystemParameters.Get<bool>("COMPRESS_SCRIPTS");

                #region Collect JS libraries and files

                List<string> scripts = new List<string>(0);

                #region External libraries - JQuery, Bootstrap, CryptoJS

                scripts.AddRange(new string[] {
                    Server.MapPath("~/scripts/jquery/jquery-3.7.0.min.js")
                    , Server.MapPath("~/scripts/jquery/jquery-migrate-3.4.1.min.js")
                    , Server.MapPath("~/scripts/jquery/jquery-ui-1.13.2.min.js")
                    , Server.MapPath("~/scripts/responsive/bootstrap.min.js")
					, Server.MapPath("~/scripts/cryptojs/spark-md5.min.js")
				});

                #endregion

                #region Common JS files

                string[] commonScripts = {
					Server.MapPath("~/scripts/common/basic-scripts.js"),
					Server.MapPath("~/scripts/common/tab-actions.js"),
					Server.MapPath("~/scripts/common/dateFormat.js"),
					Server.MapPath("~/scripts/common/json2.js"),
					Server.MapPath("~/scripts/common/ajax-helper.js"),
					Server.MapPath("~/scripts/common/page-hash.js"),
                    Server.MapPath("~/scripts/common/encrypt-url-helper.js"),
                    Server.MapPath("~/scripts/common/Treeview.js"),
                };

                string commonScriptFile = Server.MapPath("~/scripts/common/common-min.js");

                if (!compressScripts || !File.Exists(commonScriptFile))
                {
                    scripts.AddRange(commonScripts);
                }
                else
                {
                    scripts.Add(commonScriptFile);
                }

                #endregion

                #region Add script for datagrid

                /*
				 * PENDING
				 * to be reviewed - can we make it generic
				 */
                if (pageCode.In("BLOTTER", "TRAN_MGNT", "APP_UNLK", "ATTACHMENT", "USER_PARTNER", "USERS", "JPMC_ATTACHMENT", "APP_RTC", "APP_FEED_DETAILS", "APP_REALTIMECALL_LOG", "APP_FormTagsMap", "APP_FormsList", "APP_ATTESTATION", "APP_EDITATTESTATION", "STAG_WIN", "STAG_AFCT","STAG_ERRLOG", "APP_UploadForm", "APP_FormsXmlList", "APP_Formsdashboard", "APP_EditRealTimeCallSetting", "APP_FEED_CONFIGURATION", "APP_AddFeedConfiguration", "STAG_PPFL", "STAG_HISTORYLOG"))
                {
                    string minifiedScriptsFile = Server.MapPath("~/scripts/common/common2-min.js");

                    if (!compressScripts || !File.Exists(minifiedScriptsFile))
                    {
                        scripts.AddRange(new string[] {
							Server.MapPath("~/scripts/common/datagrid.js")
						});
                    }
                    else
                    {
                        scripts.Add(minifiedScriptsFile);
                    }
                }

                #endregion

                #region Wizard specific common JS files

                if (!string.IsNullOrWhiteSpace(appID))
                {
                    string minifiedWizardScriptsFile = Server.MapPath("~/scripts/wizard/wizard-min.js");

                    if (!compressScripts || !File.Exists(minifiedWizardScriptsFile))
                    {
                        scripts.AddRange(new string[] {
                            Server.MapPath("~/scripts/common/nav-tree.js"),
                            Server.MapPath("~/scripts/common/common-toolbar-actions.js"),
                            Server.MapPath("~/scripts/wizard/wizard-static-list-options.js"),
                            Server.MapPath("~/scripts/wizard/wizard-common.js"),
                            Server.MapPath("~/scripts/wizard/wizard-questions.js"),
                            Server.MapPath("~/scripts/wizard/ssn-masking.js"),
                            Server.MapPath("~/scripts/wizard/ajax-data-pool.js"),
                            Server.MapPath("~/scripts/wizard/DrivingLicenseRegex.js")
                        }); ;
                    }
                    else
                    {
                        scripts.Add(minifiedWizardScriptsFile);
                    }
                }

                #endregion

                #region Page specific static JS file

                if (!string.IsNullOrWhiteSpace(pageCode))
                {
                    pageCode = pageCode.Replace(" ", "_");
                    string pageScriptFile = string.Empty;
                    string pageSpecificFilePath = string.Empty;
                    string pageSpecificMinFilePath = string.Empty;
                    string mappedJSFolder = string.Empty;

                    DataTable dt = new DataTable();

                    if (this.Request.UrlReferrer != null)
                    {
                        try
                        {
                            SortedList<string, string> urlParameters = CommonUtils.Cryptography.EnDecryption.DecryptQueryString(this.Request.UrlReferrer.Query);

                            long transactionID = urlParameters.GetValue("TXNID").Parse<long>();

                            if (transactionID > 0)
                            {
                                using (CommonUtils.DB.DBHelper dbHelper =
                                    CommonUtils.DB.DBHelper.Create(CommonUtils.DB.DBHelper.Database.Config))
                                {
                                    dbHelper.AddParameter("TransactionID", transactionID);

                                    dt = dbHelper.GetDataTable("usp_Wizard_GetJSMappingFolder");
                                }

                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    DataRow dr = dt.Rows[0];

                                    mappedJSFolder = dr.GetValue<string>("MappedFolder");
                                }
                            }
                        }
                        catch { }
                    }

                    if (!mappedJSFolder.IsBlank() && !pageCode.In("BLOTTER", "TRAN_MGNT", "APP_UNLK") && Directory.Exists(this.Server.MapPath(string.Format("~/scripts/pages/{0}", mappedJSFolder))))
                    {
                        pageSpecificMinFilePath = string.Format("~/scripts/pages/{1}/{0}-min.js", pageCode, mappedJSFolder);
                        pageSpecificFilePath = string.Format("~/scripts/pages/{1}/{0}.js", pageCode, mappedJSFolder);
                    }
                    else
                    {
                        pageSpecificMinFilePath = string.Format("~/scripts/pages/{0}-min.js", pageCode);
                        pageSpecificFilePath = string.Format("~/scripts/pages/{0}.js", pageCode);
                    }

                    if (!pageCode.In("BLOTTER", "TRAN_MGNT", "APP_UNLK", "APP_RTC", "APP_FEED_DETAILS", "APP_REALTIMECALL_LOG", "APP_FormTagsMap", "APP_FormsList", "APP_ATTESTATION", "APP_EDITATTESTATION", "APP_UploadForm", "APP_FormsXmlList", "APP_Formsdashboard", "APP_EditRealTimeCallSetting", "APP_FEED_CONFIGURATION", "APP_AddFeedConfiguration") && this.Request.UrlReferrer.AbsolutePath.Contains("/wizard/ticket/"))
                    {
                        pageSpecificMinFilePath = string.Format("~/scripts/pages/ticket/{0}-min.js", pageCode);
                        pageSpecificFilePath = string.Format("~/scripts/pages/ticket/{0}.js", pageCode);
                    }
                    else if (!pageCode.In("BLOTTER", "TRAN_MGNT", "APP_UNLK", "APP_RTC", "APP_FEED_DETAILS", "APP_REALTIMECALL_LOG", "APP_FormTagsMap", "APP_FormsList", "APP_ATTESTATION", "APP_EDITATTESTATION", "APP_UploadForm", "APP_FormsXmlList", "APP_Formsdashboard", "APP_EditRealTimeCallSetting", "APP_FEED_CONFIGURATION", "APP_AddFeedConfiguration") && this.Request.UrlReferrer.AbsolutePath.Contains("/wizard/full/"))
                    {
                        pageSpecificMinFilePath = string.Format("~/scripts/pages/full/{0}-min.js", pageCode);
                        pageSpecificFilePath = string.Format("~/scripts/pages/full/{0}.js", pageCode);
                    }
                    //else if (!pageCode.In("BLOTTER", "TRAN_MGNT", "APP_UNLK") && this.Request.UrlReferrer.AbsolutePath.Contains(string.Format("/wizard/{0}/","hsbcd")))
                    //{
                    //    pageSpecificMinFilePath = string.Format("~/scripts/pages/{1}/{0}-min.js", pageCode, "hsbcd");
                    //    pageSpecificFilePath = string.Format("~/scripts/pages/{1}/{0}.js", pageCode, "hsbcd");
                    //}
                    else if (pageCode.In("USER_PARTNER"))
                    {
                        //pageSpecificMinFilePath = string.Format("~/scripts/pages/{1}/{0}-min.js", pageCode, "hsbcd");
                        pageSpecificFilePath = string.Format("~/scripts/pages/{0}/{1}.js", "maintenance", pageCode);
                    }
                    else if (pageCode.In("USERS"))
                    {
                        //pageSpecificMinFilePath = string.Format("~/scripts/pages/{1}/{0}-min.js", pageCode, "hsbcd");
                        pageSpecificFilePath = string.Format("~/scripts/pages/{0}/{1}.js", "maintenance", pageCode);
                    }
                    //else
                    //{
                    //    pageSpecificMinFilePath = string.Format("~/scripts/pages/{0}-min.js", pageCode);
                    //    pageSpecificFilePath = string.Format("~/scripts/pages/{0}.js", pageCode);
                    //}

                    pageScriptFile = Server.MapPath(pageSpecificMinFilePath);
                    if (!compressScripts || !File.Exists(pageScriptFile))
                    {
                        pageScriptFile = Server.MapPath(pageSpecificFilePath);
                    }

                    scripts.Add(pageScriptFile);
                }

                if (pageCode.Equals("EXPQUES", StringComparison.OrdinalIgnoreCase))
                {
                    DataTable dt = this.GetAdditionalPage(pageCode);

                    foreach (DataRow row in dt.Rows)
                    {
                        string stepCode = row.GetValue<string>("StepCode");

                        if (!compressScripts && !string.IsNullOrWhiteSpace(stepCode))
                        {
                            scripts.AddRange(new string[] {
				            Server.MapPath(string.Format("~/scripts/pages/{0}.js", stepCode.ToLower()))
                          //, Server.MapPath("~/scripts/pages/EXPQUES.js")
				            });
                        }
                        else
                        {
                            scripts.AddRange(new string[] {
				            Server.MapPath(string.Format("~/scripts/pages/{0}-min.js", stepCode.ToLower()))
                            //Server.MapPath("~/scripts/pages/EXPQUES-min.js")
				        });
                        }

                    }

                    //if (!compressScripts)
                    //{
                    //    scripts.AddRange(new string[] {
                    //           Server.MapPath("~/scripts/pages/EXPQUES.js")
                    //            });
                    //}
                    //else
                    //{
                    //    scripts.AddRange(new string[] {
                    //            Server.MapPath("~/scripts/pages/EXPQUES-min.js")
                    //        });
                    //}

                }

                #endregion

                #region Page specific dynamic JS file

                if (!string.IsNullOrWhiteSpace(appID))
                {
                    string txnFolder = Path.Combine(CommonUtils.SystemParameters.Get("TXN_DATA_FOLDER"), appID, "SysData", "scripts");

                    if (Directory.Exists(txnFolder))
                    {
                        string scriptFile = string.Empty;
                        string scriptFileMin = string.Empty;

                        if (!string.IsNullOrWhiteSpace(pageCode) && !string.IsNullOrWhiteSpace(appID))
                        {
                            scriptFile = Path.Combine(txnFolder, string.Format("{0}.js", pageCode.ToLower()));
                            scriptFileMin = Path.Combine(txnFolder, string.Format("{0}-min.js", pageCode.ToLower()));
                        }

                        if (compressScripts)
                        {
                            if (!File.Exists(scriptFileMin))
                            {
                                #region Compress page specific dynamic script

                                System.Diagnostics.Process proc = new System.Diagnostics.Process { EnableRaisingEvents = false };

                                proc.StartInfo.WorkingDirectory = txnFolder;
                                proc.StartInfo.FileName = Server.MapPath("~/scripts/packer/Packer.exe");
                                proc.StartInfo.Arguments = string.Format(" -o \"{0}\" -m packer \"{1}\"", Path.GetFileName(scriptFileMin), Path.GetFileName(scriptFile));
                                proc.StartInfo.UseShellExecute = false;
                                proc.StartInfo.CreateNoWindow = true;

                                proc.StartInfo.LoadUserProfile = true;
                                proc.StartInfo.RedirectStandardInput = false;
                                proc.StartInfo.RedirectStandardOutput = true;
                                proc.StartInfo.RedirectStandardError = true;

                                proc.Start();
                                proc.WaitForExit(5000);

                                int code = proc.ExitCode;
                                if (!proc.HasExited) proc.Kill();

                                #endregion
                            }

                            if (!File.Exists(scriptFileMin)) scriptFileMin = scriptFile;

                            scripts.Add(scriptFileMin);
                        }
                        else
                        {
                            scripts.Add(scriptFile);
                        }
                    }
                }

                #endregion

                #endregion

                #region Fetch JSON for Application Info

                string appInfoJson = string.Empty;

                if (this.Request.UrlReferrer != null)
                {
                    try
                    {
                        SortedList<string, string> urlParameters = CommonUtils.Cryptography.EnDecryption.DecryptQueryString(this.Request.UrlReferrer.Query);

                        long transactionID = urlParameters.GetValue("TXNID").Parse<long>();
                        long txnStepID = urlParameters.GetValue("StepID").Parse<long>();
                        long sessionID = urlParameters.GetValue("sid").Parse<long>();

                        if (transactionID > 0 && txnStepID > 0 && sessionID > 0)
                        {
                            using (CommonUtils.DB.DBHelper dbHelper =
                                CommonUtils.DB.DBHelper.Create(CommonUtils.DB.DBHelper.Database.Transactions))
                            {
                                dbHelper.AddParameter("TransactionID", transactionID);
                                dbHelper.AddParameter("TxnStepID", txnStepID);
                                dbHelper.AddParameter("SessionID", sessionID);

                                appInfoJson = dbHelper.ExecuteScaler<string>("usp_Wizard_GetJsonAppInfo");
                            }
                        }
                    }
                    catch { appInfoJson = string.Empty; }
                }

                #endregion

                #region Render JS contents

                if (!appInfoJson.IsBlank())
                {
                    this.Response.Write(string.Format("var AppContext = {0};", appInfoJson));
                }

                foreach (string script in scripts)
                {
                    if (File.Exists(script)) this.Response.Write(File.ReadAllText(script));
                }

                #endregion
            }

            this.Response.End();
        }
    }

    protected DataTable GetAdditionalPage(string pageCode)
    {
        SortedList<string, string> urlParameters = CommonUtils.Cryptography.EnDecryption.DecryptQueryString(this.Request.UrlReferrer.Query);

        long transactionID = urlParameters.GetValue("TXNID").Parse<long>();

        DataTable records = null;

        using (CommonUtils.DB.DBHelper dbHelper = CommonUtils.DB.DBHelper.Create(
            CommonUtils.DB.DBHelper.Database.Transactions))
        {
            dbHelper.AddParameter("TransactionID", transactionID);
            dbHelper.AddParameter("PageCode", pageCode);
            //dbHelper.AddParameter("CurrentStepID", this.CurrentStepID);
            //dbHelper.AddParameter("CurrentStepModeID", this.ApplicationInfo.CurrentWizardStepModeID);


            records = dbHelper.GetDataTable("usp_Wizard_GetExpQuestionnaireSteps");
        }

        return records;
    }
}
