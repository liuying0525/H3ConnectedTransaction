﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace Attachment_Aarchive_New.AttachmentService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="AttachmentServiceSoap", Namespace="http://tempuri.org/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Cloneable))]
    public partial class AttachmentService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback HelloWorldOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidateH3FileAuthorityOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidateUserOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetPickApplicationNoOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetAttachmentContentOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetGongHangAttachmentContentOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetAttachmentByteContentOperationCompleted;
        
        private System.Threading.SendOrPostCallback GenerateWordAttachmentOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public AttachmentService() {
            this.Url = global::Attachment_Aarchive_New.Properties.Settings.Default.Attachment_Aarchive_New_AttachmentService_AttachmentService;
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
        public event HelloWorldCompletedEventHandler HelloWorldCompleted;
        
        /// <remarks/>
        public event ValidateH3FileAuthorityCompletedEventHandler ValidateH3FileAuthorityCompleted;
        
        /// <remarks/>
        public event ValidateUserCompletedEventHandler ValidateUserCompleted;
        
        /// <remarks/>
        public event GetPickApplicationNoCompletedEventHandler GetPickApplicationNoCompleted;
        
        /// <remarks/>
        public event GetAttachmentContentCompletedEventHandler GetAttachmentContentCompleted;
        
        /// <remarks/>
        public event GetGongHangAttachmentContentCompletedEventHandler GetGongHangAttachmentContentCompleted;
        
        /// <remarks/>
        public event GetAttachmentByteContentCompletedEventHandler GetAttachmentByteContentCompleted;
        
        /// <remarks/>
        public event GenerateWordAttachmentCompletedEventHandler GenerateWordAttachmentCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/HelloWorld", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string HelloWorld() {
            object[] results = this.Invoke("HelloWorld", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void HelloWorldAsync() {
            this.HelloWorldAsync(null);
        }
        
        /// <remarks/>
        public void HelloWorldAsync(object userState) {
            if ((this.HelloWorldOperationCompleted == null)) {
                this.HelloWorldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHelloWorldOperationCompleted);
            }
            this.InvokeAsync("HelloWorld", new object[0], this.HelloWorldOperationCompleted, userState);
        }
        
        private void OnHelloWorldOperationCompleted(object arg) {
            if ((this.HelloWorldCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HelloWorldCompleted(this, new HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ValidateH3FileAuthority", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ValidateH3FileAuthority(string role, string userid) {
            object[] results = this.Invoke("ValidateH3FileAuthority", new object[] {
                        role,
                        userid});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ValidateH3FileAuthorityAsync(string role, string userid) {
            this.ValidateH3FileAuthorityAsync(role, userid, null);
        }
        
        /// <remarks/>
        public void ValidateH3FileAuthorityAsync(string role, string userid, object userState) {
            if ((this.ValidateH3FileAuthorityOperationCompleted == null)) {
                this.ValidateH3FileAuthorityOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidateH3FileAuthorityOperationCompleted);
            }
            this.InvokeAsync("ValidateH3FileAuthority", new object[] {
                        role,
                        userid}, this.ValidateH3FileAuthorityOperationCompleted, userState);
        }
        
        private void OnValidateH3FileAuthorityOperationCompleted(object arg) {
            if ((this.ValidateH3FileAuthorityCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidateH3FileAuthorityCompleted(this, new ValidateH3FileAuthorityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ValidateUser", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ValidateUser(string userCode, string password) {
            object[] results = this.Invoke("ValidateUser", new object[] {
                        userCode,
                        password});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ValidateUserAsync(string userCode, string password) {
            this.ValidateUserAsync(userCode, password, null);
        }
        
        /// <remarks/>
        public void ValidateUserAsync(string userCode, string password, object userState) {
            if ((this.ValidateUserOperationCompleted == null)) {
                this.ValidateUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidateUserOperationCompleted);
            }
            this.InvokeAsync("ValidateUser", new object[] {
                        userCode,
                        password}, this.ValidateUserOperationCompleted, userState);
        }
        
        private void OnValidateUserOperationCompleted(object arg) {
            if ((this.ValidateUserCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidateUserCompleted(this, new ValidateUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetPickApplicationNo", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetPickApplicationNo(System.DateTime StartTime, System.DateTime EndTime) {
            object[] results = this.Invoke("GetPickApplicationNo", new object[] {
                        StartTime,
                        EndTime});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetPickApplicationNoAsync(System.DateTime StartTime, System.DateTime EndTime) {
            this.GetPickApplicationNoAsync(StartTime, EndTime, null);
        }
        
        /// <remarks/>
        public void GetPickApplicationNoAsync(System.DateTime StartTime, System.DateTime EndTime, object userState) {
            if ((this.GetPickApplicationNoOperationCompleted == null)) {
                this.GetPickApplicationNoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPickApplicationNoOperationCompleted);
            }
            this.InvokeAsync("GetPickApplicationNo", new object[] {
                        StartTime,
                        EndTime}, this.GetPickApplicationNoOperationCompleted, userState);
        }
        
        private void OnGetPickApplicationNoOperationCompleted(object arg) {
            if ((this.GetPickApplicationNoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPickApplicationNoCompleted(this, new GetPickApplicationNoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetAttachmentContent", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public AttachmentHeader[] GetAttachmentContent(string applicationNo) {
            object[] results = this.Invoke("GetAttachmentContent", new object[] {
                        applicationNo});
            return ((AttachmentHeader[])(results[0]));
        }
        
        /// <remarks/>
        public void GetAttachmentContentAsync(string applicationNo) {
            this.GetAttachmentContentAsync(applicationNo, null);
        }
        
        /// <remarks/>
        public void GetAttachmentContentAsync(string applicationNo, object userState) {
            if ((this.GetAttachmentContentOperationCompleted == null)) {
                this.GetAttachmentContentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAttachmentContentOperationCompleted);
            }
            this.InvokeAsync("GetAttachmentContent", new object[] {
                        applicationNo}, this.GetAttachmentContentOperationCompleted, userState);
        }
        
        private void OnGetAttachmentContentOperationCompleted(object arg) {
            if ((this.GetAttachmentContentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAttachmentContentCompleted(this, new GetAttachmentContentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetGongHangAttachmentContent", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public AttachmentHeader[] GetGongHangAttachmentContent(string applicationNo, string Datafields) {
            object[] results = this.Invoke("GetGongHangAttachmentContent", new object[] {
                        applicationNo,
                        Datafields});
            return ((AttachmentHeader[])(results[0]));
        }
        
        /// <remarks/>
        public void GetGongHangAttachmentContentAsync(string applicationNo, string Datafields) {
            this.GetGongHangAttachmentContentAsync(applicationNo, Datafields, null);
        }
        
        /// <remarks/>
        public void GetGongHangAttachmentContentAsync(string applicationNo, string Datafields, object userState) {
            if ((this.GetGongHangAttachmentContentOperationCompleted == null)) {
                this.GetGongHangAttachmentContentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetGongHangAttachmentContentOperationCompleted);
            }
            this.InvokeAsync("GetGongHangAttachmentContent", new object[] {
                        applicationNo,
                        Datafields}, this.GetGongHangAttachmentContentOperationCompleted, userState);
        }
        
        private void OnGetGongHangAttachmentContentOperationCompleted(object arg) {
            if ((this.GetGongHangAttachmentContentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetGongHangAttachmentContentCompleted(this, new GetGongHangAttachmentContentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetAttachmentByteContent", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] GetAttachmentByteContent(string BizObjectSchemaCode, string BizObjectId, string AttachmentID) {
            object[] results = this.Invoke("GetAttachmentByteContent", new object[] {
                        BizObjectSchemaCode,
                        BizObjectId,
                        AttachmentID});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetAttachmentByteContentAsync(string BizObjectSchemaCode, string BizObjectId, string AttachmentID) {
            this.GetAttachmentByteContentAsync(BizObjectSchemaCode, BizObjectId, AttachmentID, null);
        }
        
        /// <remarks/>
        public void GetAttachmentByteContentAsync(string BizObjectSchemaCode, string BizObjectId, string AttachmentID, object userState) {
            if ((this.GetAttachmentByteContentOperationCompleted == null)) {
                this.GetAttachmentByteContentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetAttachmentByteContentOperationCompleted);
            }
            this.InvokeAsync("GetAttachmentByteContent", new object[] {
                        BizObjectSchemaCode,
                        BizObjectId,
                        AttachmentID}, this.GetAttachmentByteContentOperationCompleted, userState);
        }
        
        private void OnGetAttachmentByteContentOperationCompleted(object arg) {
            if ((this.GetAttachmentByteContentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetAttachmentByteContentCompleted(this, new GetAttachmentByteContentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GenerateWordAttachment", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GenerateWordAttachment(string applicationNum, string customername, string id, string field) {
            object[] results = this.Invoke("GenerateWordAttachment", new object[] {
                        applicationNum,
                        customername,
                        id,
                        field});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GenerateWordAttachmentAsync(string applicationNum, string customername, string id, string field) {
            this.GenerateWordAttachmentAsync(applicationNum, customername, id, field, null);
        }
        
        /// <remarks/>
        public void GenerateWordAttachmentAsync(string applicationNum, string customername, string id, string field, object userState) {
            if ((this.GenerateWordAttachmentOperationCompleted == null)) {
                this.GenerateWordAttachmentOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGenerateWordAttachmentOperationCompleted);
            }
            this.InvokeAsync("GenerateWordAttachment", new object[] {
                        applicationNum,
                        customername,
                        id,
                        field}, this.GenerateWordAttachmentOperationCompleted, userState);
        }
        
        private void OnGenerateWordAttachmentOperationCompleted(object arg) {
            if ((this.GenerateWordAttachmentCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GenerateWordAttachmentCompleted(this, new GenerateWordAttachmentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3130.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AttachmentHeader : SerializableObject {
        
        private string bizObjectIdField;
        
        private string weChatMediaIDField;
        
        private System.DateTime weChatMediaExpireTimeField;
        
        private string dingTalkMediaIDField;
        
        private string createdByField;
        
        private bool lastVersionField;
        
        private string dataFieldField;
        
        private string contentTypeField;
        
        private int contentLengthField;
        
        private string fileNameField;
        
        private System.DateTime createdTimeField;
        
        private string descriptionField;
        
        private long fileFlagField;
        
        private bool printableField;
        
        private bool downloadableField;
        
        private string bizObjectSchemaCodeField;
        
        private string storagePathField;
        
        private string storageFileNameField;
        
        private string downloadUrlField;
        
        private string modifiedByField;
        
        private System.DateTime modifiedTimeField;
        
        /// <remarks/>
        public string BizObjectId {
            get {
                return this.bizObjectIdField;
            }
            set {
                this.bizObjectIdField = value;
            }
        }
        
        /// <remarks/>
        public string WeChatMediaID {
            get {
                return this.weChatMediaIDField;
            }
            set {
                this.weChatMediaIDField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime WeChatMediaExpireTime {
            get {
                return this.weChatMediaExpireTimeField;
            }
            set {
                this.weChatMediaExpireTimeField = value;
            }
        }
        
        /// <remarks/>
        public string DingTalkMediaID {
            get {
                return this.dingTalkMediaIDField;
            }
            set {
                this.dingTalkMediaIDField = value;
            }
        }
        
        /// <remarks/>
        public string CreatedBy {
            get {
                return this.createdByField;
            }
            set {
                this.createdByField = value;
            }
        }
        
        /// <remarks/>
        public bool LastVersion {
            get {
                return this.lastVersionField;
            }
            set {
                this.lastVersionField = value;
            }
        }
        
        /// <remarks/>
        public string DataField {
            get {
                return this.dataFieldField;
            }
            set {
                this.dataFieldField = value;
            }
        }
        
        /// <remarks/>
        public string ContentType {
            get {
                return this.contentTypeField;
            }
            set {
                this.contentTypeField = value;
            }
        }
        
        /// <remarks/>
        public int ContentLength {
            get {
                return this.contentLengthField;
            }
            set {
                this.contentLengthField = value;
            }
        }
        
        /// <remarks/>
        public string FileName {
            get {
                return this.fileNameField;
            }
            set {
                this.fileNameField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime CreatedTime {
            get {
                return this.createdTimeField;
            }
            set {
                this.createdTimeField = value;
            }
        }
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public long FileFlag {
            get {
                return this.fileFlagField;
            }
            set {
                this.fileFlagField = value;
            }
        }
        
        /// <remarks/>
        public bool Printable {
            get {
                return this.printableField;
            }
            set {
                this.printableField = value;
            }
        }
        
        /// <remarks/>
        public bool Downloadable {
            get {
                return this.downloadableField;
            }
            set {
                this.downloadableField = value;
            }
        }
        
        /// <remarks/>
        public string BizObjectSchemaCode {
            get {
                return this.bizObjectSchemaCodeField;
            }
            set {
                this.bizObjectSchemaCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StoragePath {
            get {
                return this.storagePathField;
            }
            set {
                this.storagePathField = value;
            }
        }
        
        /// <remarks/>
        public string StorageFileName {
            get {
                return this.storageFileNameField;
            }
            set {
                this.storageFileNameField = value;
            }
        }
        
        /// <remarks/>
        public string DownloadUrl {
            get {
                return this.downloadUrlField;
            }
            set {
                this.downloadUrlField = value;
            }
        }
        
        /// <remarks/>
        public string ModifiedBy {
            get {
                return this.modifiedByField;
            }
            set {
                this.modifiedByField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ModifiedTime {
            get {
                return this.modifiedTimeField;
            }
            set {
                this.modifiedTimeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AttachmentHeader))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3130.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public abstract partial class SerializableObject : Cloneable {
        
        private bool serializedField;
        
        private string objectIDField;
        
        private string parentObjectIDField;
        
        private string parentPropertyNameField;
        
        private int parentIndexField;
        
        /// <remarks/>
        public bool Serialized {
            get {
                return this.serializedField;
            }
            set {
                this.serializedField = value;
            }
        }
        
        /// <remarks/>
        public string ObjectID {
            get {
                return this.objectIDField;
            }
            set {
                this.objectIDField = value;
            }
        }
        
        /// <remarks/>
        public string ParentObjectID {
            get {
                return this.parentObjectIDField;
            }
            set {
                this.parentObjectIDField = value;
            }
        }
        
        /// <remarks/>
        public string ParentPropertyName {
            get {
                return this.parentPropertyNameField;
            }
            set {
                this.parentPropertyNameField = value;
            }
        }
        
        /// <remarks/>
        public int ParentIndex {
            get {
                return this.parentIndexField;
            }
            set {
                this.parentIndexField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SerializableObject))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AttachmentHeader))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3130.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Cloneable {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void HelloWorldCompletedEventHandler(object sender, HelloWorldCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HelloWorldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal HelloWorldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void ValidateH3FileAuthorityCompletedEventHandler(object sender, ValidateH3FileAuthorityCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidateH3FileAuthorityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidateH3FileAuthorityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void ValidateUserCompletedEventHandler(object sender, ValidateUserCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ValidateUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidateUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GetPickApplicationNoCompletedEventHandler(object sender, GetPickApplicationNoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPickApplicationNoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPickApplicationNoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GetAttachmentContentCompletedEventHandler(object sender, GetAttachmentContentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetAttachmentContentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetAttachmentContentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AttachmentHeader[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AttachmentHeader[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GetGongHangAttachmentContentCompletedEventHandler(object sender, GetGongHangAttachmentContentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetGongHangAttachmentContentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetGongHangAttachmentContentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public AttachmentHeader[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((AttachmentHeader[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GetAttachmentByteContentCompletedEventHandler(object sender, GetAttachmentByteContentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetAttachmentByteContentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetAttachmentByteContentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void GenerateWordAttachmentCompletedEventHandler(object sender, GenerateWordAttachmentCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GenerateWordAttachmentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GenerateWordAttachmentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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