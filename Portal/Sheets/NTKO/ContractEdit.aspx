 <%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="ContractEdit.aspx.cs"
    Inherits="ContractEdit" %>


<%--<%@ Import Namespace="OThinker.H3.WorkItem" %>--%>
<%--<%@ Register Assembly="OThinker.H3.WorkSheet" Namespace="OThinker.H3.WorkSheet" TagPrefix="SheetControls" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headContent" Runat="Server">
    <script type="text/javascript">
        //this.opener.window.$("#Control13").val();
        // 设置 Word 控件状态
        // readOnly : 是否只读
        function setWordStatus(readOnly) {
            try {
                if (wordObject.Accept != null && wordObject.Accept) {// 是否接受所有修订
                    try {
                        TANGER_OCX_AllRevisions(true);   // 接受所有修订
                    } catch (e) { }
                }
                if (wordObject.Mark == null || !wordObject.Mark) {
                    if (sheetWordInfo.isStartUp == "0") {// 第一个节点，不做接受修订操作
                        // 设置不保留痕迹，并且接受所有修订
                        try {
                            SetReviewMode(false);            // 取消留痕
                            TANGER_OCX_AllRevisions(true);   // 接受所有修订
                        }
                        catch (e) {
                            TANGER_OCX_OBJ.SetReadOnly(false);
                            SetReviewMode(false);            // 取消留痕
                            TANGER_OCX_AllRevisions(true);   // 接受所有修订
                            TANGER_OCX_OBJ.SetReadOnly(true);
                        }
                    }
                }
                else {
                    // 设置保留痕迹
                    SetReviewMode(true);            // 设置留痕
                }
            }
            catch (e) { }
        }

        function SetControl() {
            var tangerOffObj = document.getElementById("TANGER_OCX");
            var participantId = "<%=this.Enviroment.UserValidator.User.ObjectID %>";
            if (participantId == "97ea27f1-91b8-42de-afe2-058a9671514c" || participantId == "ef9c020f-887b-467e-bb9b-7ce0302ee9bb")
            {
                //参与者
                tangerOffObj.FileSaveAs = true;
            }
            var mode = "<%=this.Enviroment.WorkItem.ItemType%>";
            if (mode == "Circulate") {
                tangerOffObj.SetReadOnly(true);
            }
            var workflowCode = "<%=this.Enviroment.WorkflowCode %>";
            if (workflowCode == "ZB_CNT_OutCooperate") {
                var saveAsMembers = "ss"; <%--"<%=GetOutCooperateSaveAs()%>";--%>
                var members = saveAsMembers.split(';');
                if (members.length > 0) {
                    for (var i = 0; i < members.length; i++) {
                        if (participantId == members[i]) {
                            tangerOffObj.FileSaveAs = true;
                            break;
                        }
                    }
                }
            }
        }
        // 保存之前事件，保存 Word 文档  
        function saveDoc() {
            var title = document.getElementById("ctl00_masterContent_hdDocTitle").value;
            // 保存 Word 文档
            sheetOffice.saveDocument(title);
            return true;
        }
        // 转换 WORD 文档为 PDF 格式的文档
        // 这个按钮最好不要在流程的第一个环节使用，因为流程可能还未发起，生成的PDF附件不能绑定到该流程
        function saveWordToPDF() {
        var fileName = "<%=this.Enviroment.InstanceData["ContractName"].Value %>";
            saveFileAsPdfToUrl(fileName, 0);
        }

        function TANGER_OCX_GetDoSignDept() {
            var contractDept = "<%=GetContactDeptName()%>";
            return contractDept;
        }

        // 套用模板方法
        function TANGER_OCX_Template() {
            //因为套用模板后,模板中的书签会出现在Word文档内,所以判断是否存在某个名称的书签来判断是否已经套用过一次模板
            if (sheetOffice.ntkoOffice.TANGER_OCX_OBJ.ActiveDocument.BookMarks.Exists("Body")) return;
            var tempId = $("select[id*=ddl_TempName]").val();
            if (typeof (tempId) == "undefined" || tempId == "-1" || tempId == "") {
                alert("未选择合同模版！");
                return;
            } else {
                //var docDot = "<%=this.Enviroment.InstanceData["ContractType"].Value %>";
                var docDot = $("select[id*=ddl_TempName]").find("option:selected").text();
                var contractDept = "<%=GetContactDeptName()%>";
                var value = {
                    mark: []
                };
                if (docDot == null) {
                    docDot = "其他合同";
                }
                var docUrl;
                if (contractDept == '铁马公司') {
                    docUrl = "../CNT_TM_DocTemplate/";
                } else if (contractDept == '汽车零部件公司') {
                    docUrl = "../CNT_QC_DocTemplate/";
                } else if (contractDept == '柴油机零部件公司') {
                    docUrl = "../CNT_CYJ_DocTemplate/";
                } else if (contractDept == '瑞泰公司') {
                    docUrl = "../CNT_RT_DocTemplate/";
                } else if (contractDept == '朗锐铸造公司') {
                    docUrl = "../CNT_LRZZ_DocTemplate/";
                } else if (contractDept == '朗锐茂达公司') {
                    docUrl = "../CNT_LRMD_DocTemplate/";
                } else if (contractDept == '资阳传动公司') {
                    docUrl = "../CNT_ZY_DocTemplate/";
                } else {
                    docUrl = "../CNT_DocTemplate/";
                }
                sheetOffice.TANGER_OCX_PrintTemplate(docUrl + docDot + ".doc", value);
            }
        }
        // 查看正文
        var viewDocument = function () {
            // 请先保存PDF文档，再进行查看正文操作
           <%-- //var attachmentId = document.getElementById("<%=hidAttachmentID.ClientID %>").value;--%>
            if (attachmentId&&attachmentId!="") {
                
            }
            else {
                alert("请先保存PDF后，再查看正文！");
                return;
            }
            window.open("<%=this.Enviroment.PortalRoot%>/ReadAttachment.aspx?AttachmentID=" + attachmentId + "&OpenMethod=1");
        };

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]);
            return "";
        }

        $(window).load(function () {
            SetControl();
            var mode = GetQueryString("Mode").toLowerCase();
            if (mode == "view") {
                $("#libtnSave").hide();
            }
        });

        function customValidate(eventType) {
            if (eventType == "2" || eventType == "1" || eventType == "3") {
                var tt = document.getElementById("TANGER_OCX");
                if (tt == null) {
                    alert("合同正文未展开!");
                    return false;
                }
                else {
                    return true;
                }
            }
            return true;
        }

        var viewProjectStatus = function () {
            var instanceId = GetQueryString("ViewInstanceId");
            //var instanceId ='<%=Enviroment.InstanceId %>';
            var contractUrl = '<%=ConfigurationManager.AppSettings["ContractUrl"] %>';
            if (instanceId!="") {
                 window.open(contractUrl+"/ProjectMag/RedirectToStatus.aspx?InstanceId=" + instanceId );
            }
            else {
                alert("流程实例不存在,请保存后再查看项目状态！");
                return;
            }
        };

    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="masterTool" Runat="Server">
   <li id="libtnSave"><a href="#" onclick="saveDoc();"><i class="panel-title-icon fa fa-save toolImage"></i><span class="toolText">保存</span></a></li>
   <li><a href="#" onclick="javascript:if(window.opener){window.opener.location.href=window.opener.location.href;}window.opener=null;window.open('','_self');window.close();"><i class="panel-title-icon fa fa-times toolImage"></i><span class="toolText">取消</span></a></li>
   <li><a href="#" onclick="viewProjectStatus();"><i class="panel-title-icon fa fa-ellipsis-v toolImage"></i><span class="toolText">项目状态</span></a></li>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="masterContent" Runat="Server">
    <div class="divContent">
        <div style="text-align: center;" class="DragContainer">
            <span class="panel-title">
                <asp:Label ID="lblTitle" runat="server" Text="" />
            </span>
        </div>
        <div class="panel-body">
            <div id="dvBaseInfo" class="dvPanel">
            <div class="divContent">
                <table id="ctl00_BodyContent_tbTable" class="tableStyle" width="100%">
                    <tbody>
                        <tr>
                            <td colspan="1">
                                 <asp:Label runat="server" Text="模版名称"></asp:Label>
                            </td>
                            <td colspan="2">
                                 <asp:DropDownList runat="server" ID="ddl_TempName" Width="98%"/>
                            </td>
                            <td colspan="3">
                                <asp:HiddenField runat="server"/>
                            </td>
                        </tr>
                         <tr>
                             <td colspan="6">
                                 <%--<SheetControls:SheetOffice ID="SheetOffice1" CABPath="../../Office/OfficeControl.cab" Width="870px" Height="900px" DataField="" TemplateDoc="Template.doc" runat="server" />--%>
                             </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            </div>
        </div>
        <!--doc名称-->
        <asp:HiddenField ID="hdDocTitle" runat="server" /> 
       <%-- <!-- PDF正文 ObjectID -->
        <SheetControls:SheetHiddenField ID="hidAttachmentID" runat="server" />
        <!-- 是否保存 PDF 文档 -->
        <SheetControls:SheetHiddenField ID="hidSavePDF" runat="server" />--%>
      <%--  <asp:HiddenField ID="hdDataField" runat="server" /><!--office正文数据项名称-->
        <asp:HiddenField ID="hdPDFDataField" runat="server" /><!--PDF数据项名称-->--%>
     </div>
   
</asp:Content>

