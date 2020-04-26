<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NTKO_TESTEdit.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.NTKO_TESTEdit" EnableEventValidation="false" MasterPageFile="~/MvcSheetNTKO.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <style type="text/css">
        .panel {
            box-shadow: 0 0 15px red;
            width: 130%;
            margin-left: -15%; 
             
        }

        #content-wrapper {
        
        padding-top:30px;
        }
    </style>
    <script type="text/javascript">

        // 保存之前事件，保存 Word 文档  
        function saveDoc() {
            var NTJO = $("div[data-datafield=ContractTemplateNTKO]").SheetUIManager();
            $("div[data-datafield=ContractTemplateNTKO]").SheetUIManager().SaveOffice();
            alert("保存成功");
            return true;
        }


        //页面加载前事件
        $.MvcSheet.PreInit = function () {
            // ChangeToPdf(); 
            //$.MvcSheet.AddAction({
            //    Action: "",
            //    Icon: "fa-coumns",
            //    Text: "打印",
            //    Datas: [],
            //    OnAction: function () {
            //    },
            //    OnActionDone: function () {
            //    },
            //    PostSheetInfo: false
            //});

            var Template = GetQueryString("Template");
            $("div[data-datafield=ContractTemplateNTKO]").attr("data-template", "/Portal/TemplateFile/" + Template + ".doc")

        };


        function Print() {
            $("#TANGER_OCX")[0].PrintOut(true)
        }



        function ChangeToPdf() {
            $.MvcSheet.Action(
                 {
                     Action: "ChangeToPdf",//后台方法名称
                     Datas: [], //输入参数，格式["{数据项名称}","string值","控件ID"]
                     LoadControlValue: false,//是否获取表单数据
                     PostSheetInfo: false, //是否获取已经改变的表单数据
                     OnActionDone: function (e) {
                         //执行回调事件
                         //alert(e);

                         if (e) {

                         }
                     }
                 });
        }

        $.MvcSheet.Loaded = function (sheetInfo) {
            TANGER_OCX_OBJ = document.getElementById("TANGER_OCX");
            TANGER_OCX_OBJ.IsShowToolMenu = false;
            TANGER_OCX_OBJ.IsNoCopy = false;//控件中的文档是否禁止拷贝
            TANGER_OCX_OBJ.IsStrictNoCopy = false;//是否严格禁止拷贝
            TANGER_OCX_OBJ.FileSaveAs = false;
            TANGER_OCX_OBJ.FileSave = false;
            TANGER_OCX_OBJ.FileOpen = false;
            TANGER_OCX_OBJ.FileNew = false;
            TANGER_OCX_OBJ.ToolBars = false;

            TANGER_OCX_OBJ.Menubar = false;
            TANGER_OCX_OBJ.CustomToolBar = false;
            //    TANGER_OCX_OBJ.IsShowEditMenu = false;
            //    TANGER_OCX_OBJ.IsShowInsertMenu = false;
            //    TANGER_OCX_OBJ.IsShowHelpMenu = false;
            //    TANGER_OCX_OBJ.CancelWordRightClick = false; 

            //    //TANGER_OCX_OBJ.ShowDialog("4");
            //    //TANGER_OCX_OBJ.CustomMenuStr = 0;
            //    //TANGER_OCX_OBJ.ShowCommandBar("7", false);
            //    //TANGER_OCX_OBJ.ShowCommandBar("8", false);
            //    //TANGER_OCX_OBJ.ShowCommandBar("9", false);
            //    //TANGER_OCX_OBJ.ShowCommandBar("12", false);
            //    //TANGER_OCX_OBJ.ShowCommandBar("13", false);
        }
        // 设置文档是否为只读模式
        var SetReadOnly = function (readonly) {
            $("#TANGER_OCX")[0].SetReadOnly(readonly);
        }



        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]);
            return "";
        }


</script>
</asp:Content>
<%--<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>--%>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenuNTKO" runat="Server">
    <%-- <li><a href="#" onclick="saveDoc();"><i class="panel-title-icon fa fa-save toolImage"></i><span class="toolText">保存</span></a></li>--%>
    <li><a href="#" onclick="javascript:window.opener=null;window.open('','_self');window.close();"><i class="panel-title-icon fa fa-times toolImage"></i><span class="toolText">关闭</span></a></li>
    <li><a href="#" onclick="Print();"><i class="panel-title-icon fa fa-save toolImage"></i><span class="toolText">打印</span></a></li>

</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">


 <%--   <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">合同模板</label>
    </div>--%>
    <div class="panel-body sheetContainer">

        <%-- <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
        </div>--%>
        <div class="divContent" id="divSheet">

            <div id="collpsAtt" class="divContent panel-collapse collapse in">
                <div class="row">

                    <div id="control17" class="col-md-12" style="z-index: -1">

                        <%--<div data-datafield="ContractTemplateNTKO" data-type="SheetOffice" data-template="/Portal/Sheets/模版/个人零售贷款合同_final.doc" id="ctl992158" class="" data-cabpath="/Portal/Office/OfficeControl.cab" data-classid="A64E3073-2016-4baf-A89D-FFE1FAA10EC0" data-productversion="5,0,3,0" data-productcaption="" data-productkey=""></div>    data-template="/Portal/Sheets/模版/个人零售贷款合同.doc"   --%>
                        <div data-datafield="ContractTemplateNTKO" data-type="SheetOffice" id="ctl175708" class="" style="margin-left: -5%; width: 110%;height:100%" data-productkey="3430AB592E7096D6522268A648A8D79B831A6736" data-productcaption="上海市文化广播影视监测中心" data-productversion="5,0,2,9" data-classid="A64E3073-2016-4baf-A89D-FFE1FAA10EC0" data-officewidth="100%">
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </div>
</asp:Content>
