<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FIApplication_Mobile.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.FIApplication_Mobile" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <script type="text/javascript" src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript" src="js/FIApplication.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript" src="js/Validate.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript" src="/Portal/Custom/js/ocr.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript">
        function appType_change() {
            $.MvcSheetUI.SetControlValue("APPLICATION_TYPE_NAME", $.MvcSheetUI.GetElement("APPLICATION_TYPE_CODE").find("option:selected").text());
        }

        $.MvcSheet.PreInit = function () {
            //移动端不允许提交
            $.MvcSheetUI.SheetInfo.PermittedActions.Submit = false;
            $.MvcSheetUI.SheetInfo.PermittedActions.Forward = false;
            var app_num = $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_NUMBER.V;
            if (app_num && app_num.indexOf("Br-") == 0) {//去掉取消流程按钮
                $.MvcSheetUI.SheetInfo.PermittedActions.CancelInstance = false;
            }
        }

    </script>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">零售贷款申请</label>
    </div>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-right bannerTitle">
            <label id="divBasicInfo" data-en_us="Basic information">基本信息</label>
        </div>
        <div class="divContent">
            <div class="row">
                <div id="divFullNameTitle" class="col-md-2">
                    <label id="lblFullNameTitle" data-type="SheetLabel" data-datafield="Originator.UserName" data-en_us="Originator" data-bindtype="OnlyVisiable" style="">发起人</label>
                </div>
                <div id="divFullName" class="col-md-4">
                    <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style="">
                    </label>
                </div>
                <div id="divOriginateDateTitle" class="col-md-2">
                    <label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                </div>
                <div id="divOriginateDate" class="col-md-4">
                    <label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style="">
                    </label>
                </div>
            </div>
            <div class="row">
                <div id="divOriginateOUNameTitle" class="col-md-2">
                    <label id="lblOriginateOUNameTitle" data-type="SheetLabel" data-datafield="Originator.OUName" data-en_us="Originate OUName" data-bindtype="OnlyVisiable" style="">所属组织</label>
                </div>
                <div id="divOriginateOUName" class="col-md-4">
                    <!--					<label id="lblOriginateOUName" data-type="SheetLabel" data-datafield="Originator.OUName" data-bindtype="OnlyData">
<span class="OnlyDesigner">Originator.OUName</span>
					</label>-->
                    <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style="">
                    </select>
                </div>
                <div id="divSequenceNoTitle" class="col-md-2">
                    <label id="lblSequenceNoTitle" data-type="SheetLabel" data-datafield="SequenceNo" data-en_us="SequenceNo" data-bindtype="OnlyVisiable" style="">流水号</label>
                </div>
                <div id="divSequenceNo" class="col-md-4">
                    <label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style="">
                    </label>
                </div>
            </div>
        </div>

        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
        </div>
        <div class="divContent" style="padding-right: 0%">
            <div class="row">
                <div id="title461" class="col-md-2">
                    <span id="Label413" data-type="SheetLabel" data-datafield="Cbankname" style="">客户银行</span>
                </div>
                <div id="control461" class="col-md-4">
                    <input id="Control413" type="text" data-datafield="Cbankname" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'50')">
                </div>
                <div id="Div1" class="col-md-2">
                    <span id="Span1" data-type="SheetLabel" data-datafield="Caccountnum" style="">账户号</span>
                </div>
                <div id="Div3" class="col-md-4">
                    <input id="inp_Caccountnum" oninput="textnum(this)" type="text" data-datafield="Caccountnum" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'50')">
                </div>
            </div>
        </div>

        <div class="nav-icon fa fa-chevron-right bannerTitle">
            <label data-en_us="Basic information">借款人信息</label>
        </div>
        <div class="divContent" id="myindex">
            <div class="SheetGridView hasBorderBottom">
                <div class="item-content tab-mode">
                    <div class="item-index">
                        <div class="buttons-left">
                            <ion-scroll class="scroll scroll-view ionic-scroll scroll-x" direction="x" scrollbar-x="false" style="display: block;">
                        <div class="scroll" style="transform: translate3d(0px, 0px, 0px) scale(1);">
                            
                        </div>
                    </ion-scroll>
                        </div>
                        <div class="buttons-right"><span style="font-size: 35px;" class="addrow">+</span></div>
                    </div>
                </div>
            </div>
        </div>

        <div class="divContent" id="divSheet">
            <div class="row hidden">
                <div id="title50" class="col-md-2">
                    <span id="Label58" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_CODE" style="">申请类型</span>
                </div>
                <div id="control50" class="col-md-4">
                    <select id="Control58" data-datafield="APPLICATION_TYPE_CODE" data-type="SheetDropDownList" style="" data-masterdatacategory="申请类型" data-onchange="appType_change()" data-queryable="false"></select>
                </div>
            </div>
            
            <div class="row tableContent">
                <div id="title75" class="col-md-2">
                    <span id="Label79" data-type="SheetLabel" data-datafield="APPLICANT_TYPE" style="">人员关系表</span>
                </div>
                <div id="control75" class="col-md-10">
                    <table id="Control79" data-datafield="APPLICANT_TYPE" data-type="SheetGridView" class="SheetGridView"
                        data-defaultrowcount="0">
                        <tbody>

                            <tr class="header">
                                <td id="Control79_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control79_Header4" data-datafield="APPLICANT_TYPE.IDENTIFICATION_CODE1">
                                    <label id="Control79_Label4" data-datafield="APPLICANT_TYPE.IDENTIFICATION_CODE1" data-type="SheetLabel" style="">IDENTIFICATION_CODE</label>
                                </td>
                                <td id="Control79_Header5" data-datafield="APPLICANT_TYPE.APPLICANT_TYPE">
                                    <label id="Control79_Label5" data-datafield="APPLICANT_TYPE.APPLICANT_TYPE" data-type="SheetLabel" style="">APPLICANT_TYPE</label>
                                </td>
                                <td id="Control79_Header7" data-datafield="APPLICANT_TYPE.GUARANTOR_TYPE">
                                    <label id="Control79_Label7" data-datafield="APPLICANT_TYPE.GUARANTOR_TYPE" data-type="SheetLabel" style="">GUARANTOR_TYPE</label>
                                </td>
                                <td id="Control79_Header8" data-datafield="APPLICANT_TYPE.MAIN_APPLICANT">
                                    <label id="Control79_Label8" data-datafield="APPLICANT_TYPE.MAIN_APPLICANT" data-type="SheetLabel" style="">MAIN_APPLICANT</label>
                                </td>
                                <td id="Control79_Header9" data-datafield="APPLICANT_TYPE.IS_INACTIVE_IND">
                                    <label id="Control79_Label9" data-datafield="APPLICANT_TYPE.IS_INACTIVE_IND" data-type="SheetLabel" style="">IS_INACTIVE_IND</label>
                                </td>
                                <td id="Control79_Header10" data-datafield="APPLICANT_TYPE.NAME1">
                                    <label id="Control79_Label10" data-datafield="APPLICANT_TYPE.NAME1" data-type="SheetLabel" style="">NAME</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control79_Option" class="rowOption"></td>
                                <td data-datafield="APPLICANT_TYPE.IDENTIFICATION_CODE1">
                                    <input id="Control79_ctl4" type="text" data-datafield="APPLICANT_TYPE.IDENTIFICATION_CODE1" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_TYPE.APPLICANT_TYPE">
                                    <input id="Control79_ctl5" type="text" data-datafield="APPLICANT_TYPE.APPLICANT_TYPE" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_TYPE.GUARANTOR_TYPE">
                                    <input id="Control79_ctl7" type="text" data-datafield="APPLICANT_TYPE.GUARANTOR_TYPE" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_TYPE.MAIN_APPLICANT">
                                    <input id="Control79_ctl8" type="text" data-datafield="APPLICANT_TYPE.MAIN_APPLICANT" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_TYPE.IS_INACTIVE_IND">
                                    <input id="Control79_ctl9" type="text" data-datafield="APPLICANT_TYPE.IS_INACTIVE_IND" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_TYPE.NAME1">
                                    <input id="Control79_ctl10" type="text" data-datafield="APPLICANT_TYPE.NAME1" data-type="SheetTextBox" style="">
                                </td>
                                <td class="rowOption">
                                    <a class="delete">
                                        <div class="fa fa-minus">
                                        </div>
                                    </a>
                                    <a class="insert">
                                        <div class="fa fa-arrow-down">
                                        </div>
                                    </a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row tableContent">
                <div id="control77" class="col-md-12">
                    <table id="Control80" data-datafield="APPLICANT_DETAIL" data-type="SheetGridView" class="SheetGridView"
                        data-defaultrowcount="0" data-onremoved="app_type_remove(this,arguments);">
                        <tbody>

                            <tr class="header">
                                <td id="Control80_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control80_Header4" data-datafield="APPLICANT_DETAIL.IDENTIFICATION_CODE2" class="hidden">
                                    <label id="Control80_Label4" data-datafield="APPLICANT_DETAIL.IDENTIFICATION_CODE2" data-type="SheetLabel" style="">IDENTIFICATION_CODE</label>
                                </td>
                                <td id="Control80_Header78" data-datafield="APPLICANT_DETAIL.FIRST_THI_NME">
                                    <label id="Control80_Label78" data-datafield="APPLICANT_DETAIL.FIRST_THI_NME" data-type="SheetLabel" style="">姓名（中文）</label>
                                </td>
                                <td id="Control80_Header44" data-datafield="APPLICANT_DETAIL.ID_CARD_TYP">
                                    <label id="Control80_Label44" data-datafield="APPLICANT_DETAIL.ID_CARD_TYP" data-type="SheetLabel" style="">证件类型</label>
                                </td>
                                <td id="Control80_Header45" data-datafield="APPLICANT_DETAIL.ID_CARD_NBR">
                                    <label id="Control80_Label45" data-datafield="APPLICANT_DETAIL.ID_CARD_NBR" data-type="SheetLabel" style="">证件号码</label>
                                </td>
                                <td id="Control80_Header60" data-datafield="APPLICANT_DETAIL.HUKOU_CDE">
                                    <label id="Control80_Label60" data-datafield="APPLICANT_DETAIL.HUKOU_CDE" data-type="SheetLabel" style="">户口所在地</label>
                                </td>
                                <td id="Control80_Header25" data-datafield="APPLICANT_DETAIL.SEX">
                                    <label id="Control80_Label25" data-datafield="APPLICANT_DETAIL.SEX" data-type="SheetLabel" style="">性别</label>
                                </td>
                                
                                <td id="Control80_Header16" data-datafield="APPLICANT_DETAIL.DATE_OF_BIRTH">
                                    <label id="Control80_Label16" data-datafield="APPLICANT_DETAIL.DATE_OF_BIRTH" data-type="SheetLabel" style="">出生日期</label>
                                </td>
                                <td id="Control80_Header19" data-datafield="APPLICANT_DETAIL.AGE_IN_YEAR">
                                    <label id="Control80_Label19" data-datafield="APPLICANT_DETAIL.AGE_IN_YEAR" data-type="SheetLabel" style="">年龄（年）</label>
                                </td>
                                <td id="Control80_Header20" data-datafield="APPLICANT_DETAIL.AGE_IN_MONTH">
                                    <label id="Control80_Label20" data-datafield="APPLICANT_DETAIL.AGE_IN_MONTH" data-type="SheetLabel" style="">年龄（月）</label>
                                </td>
                                <td id="Control80_Header73" data-datafield="APPLICANT_DETAIL.NATION_CDE">
                                    <label id="Control80_Label73" data-datafield="APPLICANT_DETAIL.NATION_CDE" data-type="SheetLabel" style="">民族</label>
                                </td>
                                <td id="Control80_Header74" data-datafield="APPLICANT_DETAIL.ISSUING_AUTHORITY">
                                    <label id="Control80_Label74" data-datafield="APPLICANT_DETAIL.ISSUING_AUTHORITY" data-type="SheetLabel" style="">签发机关</label>
                                </td>
                                <td id="Control80_Header46" data-datafield="APPLICANT_DETAIL.ID_CARDISSUE_DTE">
                                    <label id="Control80_Label46" data-datafield="APPLICANT_DETAIL.ID_CARDISSUE_DTE" data-type="SheetLabel" style="">证件发行日</label>
                                </td>
                                <td id="Control80_Header47" data-datafield="APPLICANT_DETAIL.ID_CARDEXPIRY_DTE">
                                    <label id="Control80_Label47" data-datafield="APPLICANT_DETAIL.ID_CARDEXPIRY_DTE" data-type="SheetLabel" style="">证件到期日</label>
                                </td>
                                <%--驾驶证信息--%>
                                <td id="Control80_Header15" data-datafield="APPLICANT_DETAIL.LICENSE_NUMBER">
                                    <label id="Control80_Label15" data-datafield="APPLICANT_DETAIL.LICENSE_NUMBER" data-type="SheetLabel" style="">驾照号码</label>
                                </td>
                                <td id="Control80_Header72" data-datafield="APPLICANT_DETAIL.DRIVING_LICENSE_CODE">
                                    <label id="Control80_Label72" data-datafield="APPLICANT_DETAIL.DRIVING_LICENSE_CODE" data-type="SheetLabel" style="">驾照状态</label>
                                </td>
                                <td id="Control80_Header17" data-datafield="APPLICANT_DETAIL.LICENSE_EXPIRY_DATE">
                                    <label id="Control80_Label17" data-datafield="APPLICANT_DETAIL.LICENSE_EXPIRY_DATE" data-type="SheetLabel" style="">驾照到期日</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control80_Option" class="rowOption"></td>
                                <td data-datafield="APPLICANT_DETAIL.IDENTIFICATION_CODE2" class="hidden">
                                    <input id="Control80_ctl4" type="text" data-datafield="APPLICANT_DETAIL.IDENTIFICATION_CODE2" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.FIRST_THI_NME">
                                    <input id="Control80_ctl78" type="text" data-datafield="APPLICANT_DETAIL.FIRST_THI_NME" data-type="SheetTextBox" style=""
                                        data-onchange="setApplicationTypeName_Mobile(this,'I')">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.ID_CARD_TYP">
                                    <select data-datafield="APPLICANT_DETAIL.ID_CARD_TYP" data-type="SheetDropDownList" id="ctl604410_control43" style=""
                                        data-masterdatacategory="证件类型" data-queryable="false" data-onchange="IdentifyCodeChange(this)">
                                    </select>
                                <td data-datafield="APPLICANT_DETAIL.ID_CARD_NBR">
                                    <input type="text" data-datafield="APPLICANT_DETAIL.ID_CARD_NBR" data-type="SheetTextBox" id="ctl604410_control44" style="" 
                                        data-onchange="IdentifyCodeChange(this)" onkeyup="this.value=strMaxLength(this.value,'40')">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.HUKOU_CDE">
                                    <select data-datafield="APPLICANT_DETAIL.HUKOU_CDE" data-type="SheetDropDownList" id="ctl604410_control62" style=""
                                        data-masterdatacategory="户口所在地" data-queryable="false" data-displayemptyitem="true">
                                    </select>
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.SEX">
                                    <select data-datafield="APPLICANT_DETAIL.SEX" data-type="SheetDropDownList" id="ctl604410_control24" style=""
                                        data-masterdatacategory="性别" data-queryable="false" data-onchange="sex_chg(this)">
                                    </select>
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.DATE_OF_BIRTH">
                                    <input id="Control80_ctl16" type="text" data-datafield="APPLICANT_DETAIL.DATE_OF_BIRTH" data-type="SheetTime" style=""
                                        data-defaultvalue="" data-onchange="birthdayChange(this)" data-maxvalue="CurrentTime" data-minvalue="1900-01-01">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.AGE_IN_YEAR">
                                    <input id="Control80_ctl19" type="text" data-datafield="APPLICANT_DETAIL.AGE_IN_YEAR" disabled data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.AGE_IN_MONTH">
                                    <input id="Control80_ctl20" type="text" data-datafield="APPLICANT_DETAIL.AGE_IN_MONTH" disabled data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.NATION_CDE">
                                    <select data-datafield="APPLICANT_DETAIL.NATION_CDE" data-type="SheetDropDownList" id="ctl604410_control78" style=""
                                        data-masterdatacategory="民族" data-queryable="false">
                                    </select>
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.ISSUING_AUTHORITY">
                                    <textarea id="Control80_ctl74" data-datafield="APPLICANT_DETAIL.ISSUING_AUTHORITY" data-type="SheetRichTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'500')">									</textarea>
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.ID_CARDISSUE_DTE">
                                    <input id="Control80_ctl46" type="text" data-datafield="APPLICANT_DETAIL.ID_CARDISSUE_DTE" data-type="SheetTime" style="" data-defaultvalue="">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.ID_CARDEXPIRY_DTE">
                                    <input id="Control80_ctl47" type="text" data-datafield="APPLICANT_DETAIL.ID_CARDEXPIRY_DTE" data-type="SheetTime" style="" data-defaultvalue=""
                                        data-onchange="id_card_date_chg(this)">
                                </td>
                                <%-- 驾驶证信息 --%>
                                <td data-datafield="APPLICANT_DETAIL.LICENSE_NUMBER">
                                    <input id="Control80_ctl15" type="text" data-datafield="APPLICANT_DETAIL.LICENSE_NUMBER" data-type="SheetTextBox" style=""
                                         data-onchange="licenseNumberCheck(this)" onkeyup="this.value=strMaxLength(this.value,'18')">
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.DRIVING_LICENSE_CODE">
                                    <select data-datafield="APPLICANT_DETAIL.DRIVING_LICENSE_CODE" data-type="SheetDropDownList" id="ctl604410_control75" style=""
                                        data-masterdatacategory="驾照状态" data-queryable="false" data-displayemptyitem="true">
                                    </select>
                                </td>
                                <td data-datafield="APPLICANT_DETAIL.LICENSE_EXPIRY_DATE">
                                    <input id="Control80_ctl17" type="text" data-datafield="APPLICANT_DETAIL.LICENSE_EXPIRY_DATE" data-type="SheetTime" style="" data-defaultvalue="">
                                </td>

                                <td class="rowOption">
                                    <a class="delete">
                                        <div class="fa fa-minus">
                                        </div>
                                    </a>
                                    <a class="insert">
                                        <div class="fa fa-arrow-down">
                                        </div>
                                    </a>
                                </td>
                            </tr>

                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row tableContent">
                <div id="control81" class="col-md-12">
                    <table id="Control82" data-datafield="ADDRESS" data-type="SheetGridView" class="SheetGridView"
                        data-defaultrowcount="0">
                        <tbody>

                            <tr class="header">
                                <td id="Control82_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control82_Header4" data-datafield="ADDRESS.ADDRESS_CODE" class="hidden">
                                    <label id="Control82_Label4" data-datafield="ADDRESS.ADDRESS_CODE" data-type="SheetLabel" style="">ADDRESS_CODE</label>
                                </td>
                                <td id="Control82_Header5" data-datafield="ADDRESS.IDENTIFICATION_CODE4" class="hidden">
                                    <label id="Control82_Label5" data-datafield="ADDRESS.IDENTIFICATION_CODE4" data-type="SheetLabel" style="">IDENTIFICATION_CODE</label>
                                </td>
                                <td id="Control82_Header39" data-datafield="ADDRESS.ADDRESS_ID">
                                    <label id="Control82_Label39" data-datafield="ADDRESS.ADDRESS_ID" data-type="SheetLabel" style="">户籍地址</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control82_Option" class="rowOption"></td>
                                <td data-datafield="ADDRESS.ADDRESS_CODE" class="hidden">
                                    <input id="Control82_ctl4" type="text" data-datafield="ADDRESS.ADDRESS_CODE" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="ADDRESS.IDENTIFICATION_CODE4" class="hidden">
                                    <input id="Control82_ctl5" type="text" data-datafield="ADDRESS.IDENTIFICATION_CODE4" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="ADDRESS.ADDRESS_ID">
                                    <textarea id="Control82_ctl39" data-datafield="ADDRESS.ADDRESS_ID" data-type="SheetRichTextBox" style="">									</textarea>
                                </td>
                                <td class="rowOption">
                                    <a class="delete">
                                        <div class="fa fa-minus">
                                        </div>
                                    </a>
                                    <a class="insert">
                                        <div class="fa fa-arrow-down">
                                        </div>
                                    </a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="row hidden">
                <div id="title94" class="col-md-2">
                    <span id="Label89" data-type="SheetLabel" data-datafield="bp_id" style="">bp_id</span>
                </div>
                <div id="control94" class="col-md-4">
                    <input id="Control89" type="text" data-datafield="bp_id" data-type="SheetTextBox" style="">
                </div>
                <div id="title227" class="col-md-2">
                    <span id="Label178" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_NAME" style="">申请类型名称</span>
                </div>
                <div id="control227" class="col-md-4">
                    <input id="Control178" type="text" data-datafield="APPLICATION_TYPE_NAME" data-type="SheetTextBox" style="">
                </div>
            </div>

            <div class="row hidden">
                <div id="title228" class="col-md-2">
                    <span id="Label179" data-type="SheetLabel" data-datafield="APPLICATION_NAME" style="">申请人名称</span>
                </div>
                <div id="control228" class="col-md-4">
                    <input id="Control179" type="text" data-datafield="APPLICATION_NAME" data-type="SheetTextBox" style="">
                </div>
                <div id="title1228" class="col-md-2">
                    <span id="Label1179" data-type="SheetLabel" data-datafield="APPLICATION_DATE" style="">APPLICATION_DATE</span>
                </div>
                <div id="contro1l228" class="col-md-4">
                    <input id="Control1179" type="text" data-datafield="APPLICATION_DATE" data-type="SheetTextBox" style="">
                </div>
            </div>

            <div class="row hidden">
                <div id="title2738" class="col-md-2">
                    <span id="Label17329" data-type="SheetLabel" data-datafield="USER_NAME" style="">USER_NAME</span>
                </div>
                <div id="control24228" class="col-md-4">
                    <input id="Control1742349" type="text" data-datafield="USER_NAME" data-type="SheetTextBox" style="" data-defaultvalue="{Originator.LoginName}">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
