﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.18444.
// 
#pragma warning disable 1591

namespace com.drollic.app.dreamer.core.com.drollic.app.dreamer.webservices {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="DreamerWebServiceBinding", Namespace="http://webservices.dreamer.app.drollic.com")]
    public partial class DreamerWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetDreamerStatsOperationCompleted;
        
        private System.Threading.SendOrPostCallback RecordDreamOperationCompleted;
        
        private System.Threading.SendOrPostCallback RecordSubmissionOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDreamerSubmissionEmailOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public DreamerWebService() {
            this.Url = global::com.drollic.app.dreamer.core.Properties.Settings.Default.com_drollic_app_dreamer_core_com_drollic_app_dreamer_webservices_DreamerWebService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetDreamerStatsCompletedEventHandler GetDreamerStatsCompleted;
        
        /// <remarks/>
        public event RecordDreamCompletedEventHandler RecordDreamCompleted;
        
        /// <remarks/>
        public event RecordSubmissionCompletedEventHandler RecordSubmissionCompleted;
        
        /// <remarks/>
        public event GetDreamerSubmissionEmailCompletedEventHandler GetDreamerSubmissionEmailCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerWebServices.php/GetDre" +
            "amerStats", RequestNamespace="http://webservices.dreamer.app.drollic.com", ResponseNamespace="http://webservices.dreamer.app.drollic.com")]
        [return: System.Xml.Serialization.SoapElementAttribute("Result")]
        public string GetDreamerStats(string macaddress) {
            object[] results = this.Invoke("GetDreamerStats", new object[] {
                        macaddress});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetDreamerStatsAsync(string macaddress) {
            this.GetDreamerStatsAsync(macaddress, null);
        }
        
        /// <remarks/>
        public void GetDreamerStatsAsync(string macaddress, object userState) {
            if ((this.GetDreamerStatsOperationCompleted == null)) {
                this.GetDreamerStatsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDreamerStatsOperationCompleted);
            }
            this.InvokeAsync("GetDreamerStats", new object[] {
                        macaddress}, this.GetDreamerStatsOperationCompleted, userState);
        }
        
        private void OnGetDreamerStatsOperationCompleted(object arg) {
            if ((this.GetDreamerStatsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDreamerStatsCompleted(this, new GetDreamerStatsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerWebServices.php/Record" +
            "Dream", RequestNamespace="http://webservices.dreamer.app.drollic.com", ResponseNamespace="http://webservices.dreamer.app.drollic.com")]
        [return: System.Xml.Serialization.SoapElementAttribute("Result")]
        public string RecordDream(string macaddress) {
            object[] results = this.Invoke("RecordDream", new object[] {
                        macaddress});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void RecordDreamAsync(string macaddress) {
            this.RecordDreamAsync(macaddress, null);
        }
        
        /// <remarks/>
        public void RecordDreamAsync(string macaddress, object userState) {
            if ((this.RecordDreamOperationCompleted == null)) {
                this.RecordDreamOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRecordDreamOperationCompleted);
            }
            this.InvokeAsync("RecordDream", new object[] {
                        macaddress}, this.RecordDreamOperationCompleted, userState);
        }
        
        private void OnRecordDreamOperationCompleted(object arg) {
            if ((this.RecordDreamCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RecordDreamCompleted(this, new RecordDreamCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerWebServices.php/Record" +
            "Submission", RequestNamespace="http://webservices.dreamer.app.drollic.com", ResponseNamespace="http://webservices.dreamer.app.drollic.com")]
        [return: System.Xml.Serialization.SoapElementAttribute("Result")]
        public string RecordSubmission(string macaddress) {
            object[] results = this.Invoke("RecordSubmission", new object[] {
                        macaddress});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void RecordSubmissionAsync(string macaddress) {
            this.RecordSubmissionAsync(macaddress, null);
        }
        
        /// <remarks/>
        public void RecordSubmissionAsync(string macaddress, object userState) {
            if ((this.RecordSubmissionOperationCompleted == null)) {
                this.RecordSubmissionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRecordSubmissionOperationCompleted);
            }
            this.InvokeAsync("RecordSubmission", new object[] {
                        macaddress}, this.RecordSubmissionOperationCompleted, userState);
        }
        
        private void OnRecordSubmissionOperationCompleted(object arg) {
            if ((this.RecordSubmissionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RecordSubmissionCompleted(this, new RecordSubmissionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://www.drollic.com/projects/dreamer/webservices/DreamerWebServices.php/GetDre" +
            "amerSubmissionEmail", RequestNamespace="http://webservices.dreamer.app.drollic.com", ResponseNamespace="http://webservices.dreamer.app.drollic.com")]
        [return: System.Xml.Serialization.SoapElementAttribute("Result")]
        public string GetDreamerSubmissionEmail(string macaddress) {
            object[] results = this.Invoke("GetDreamerSubmissionEmail", new object[] {
                        macaddress});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetDreamerSubmissionEmailAsync(string macaddress) {
            this.GetDreamerSubmissionEmailAsync(macaddress, null);
        }
        
        /// <remarks/>
        public void GetDreamerSubmissionEmailAsync(string macaddress, object userState) {
            if ((this.GetDreamerSubmissionEmailOperationCompleted == null)) {
                this.GetDreamerSubmissionEmailOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDreamerSubmissionEmailOperationCompleted);
            }
            this.InvokeAsync("GetDreamerSubmissionEmail", new object[] {
                        macaddress}, this.GetDreamerSubmissionEmailOperationCompleted, userState);
        }
        
        private void OnGetDreamerSubmissionEmailOperationCompleted(object arg) {
            if ((this.GetDreamerSubmissionEmailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDreamerSubmissionEmailCompleted(this, new GetDreamerSubmissionEmailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void GetDreamerStatsCompletedEventHandler(object sender, GetDreamerStatsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDreamerStatsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDreamerStatsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void RecordDreamCompletedEventHandler(object sender, RecordDreamCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RecordDreamCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RecordDreamCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void RecordSubmissionCompletedEventHandler(object sender, RecordSubmissionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RecordSubmissionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RecordSubmissionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void GetDreamerSubmissionEmailCompletedEventHandler(object sender, GetDreamerSubmissionEmailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDreamerSubmissionEmailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDreamerSubmissionEmailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591