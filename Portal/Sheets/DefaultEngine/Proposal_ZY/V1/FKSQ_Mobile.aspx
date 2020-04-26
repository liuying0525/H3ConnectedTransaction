<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FKSQ_Mobile.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.FKSQ_Mobile" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="/Portal/jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <link type="text/css" href="css/common.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" rel="stylesheet" />
    <script src="/Portal/jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
    <script src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" type="text/javascript"></script>
    <script src="js/Validate.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" type="text/javascript"></script>
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
        }

        #divattachment .fa-download {
            display: none !important;
        }

        #divattachment #Control429 .fa-download {
            display: inline-block !important;
        }

        #ulfjll {
            margin: 0 auto;
            font-size: 0;
            white-space: nowrap;
            overflow: auto;
        }

            #ulfjll li {
                display: inline-block;
                width: 32%;
                margin-left: 1%;
                padding-top: 1%;
            }

                #ulfjll li img {
                    width: 100%;
                }
    </style>
    <script src="/Portal/WFRes/_Scripts/TemplatePrint.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <script type="text/javascript" src="/Portal/Custom/js/ocr.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">个人汽车贷款申请</label>
    </div>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divBaseInfo',this)">
            <label data-en_us="Basic information">基本信息</label>
        </div>
        <div class="divContent" id="divSheet" style="padding-right: 0%">
            <div class="row">
                <div id="title390" class="col-md-2">
                    <span id="Label470" data-type="SheetLabel" data-datafield="APPLICATION_NUMBER" style="">贷款申请编号</span>
                </div>
                <div id="Div600" class="col-md-4">
                    <input id="Control470" type="text" data-datafield="APPLICATION_NUMBER" disabled data-type="SheetTextBox" style="">
                </div>
                <div id="divOriginateDateTitle" class="col-md-2">
                    <label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">日期</label>
                </div>
                <div id="divOriginateDate" class="col-md-4">
                    <label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style="">
                    </label>
                </div>
            </div>
            <div class="row">
                <div id="title1" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="BP_COMPANY_ID" style="">申请贷款公司</span>
                </div>
                <div id="control1" class="col-md-4">
                    <select id="Text1" data-datafield="BP_COMPANY_ID" data-type="SheetDropDownList" data-masterdatacategory="申请贷款公司" style="" data-queryable="false"></select>
                </div>
                <div id="title50" class="col-md-2">
                    <span id="Label58" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_CODE" style="">申请类型</span>
                </div>
                <div id="Div75" class="col-md-4">
                    <select id="Control58" data-datafield="APPLICATION_TYPE_CODE" data-type="SheetDropDownList" style="" data-masterdatacategory="申请类型" data-queryable="false"></select>
                </div>
            </div>
            <div class="row">
                <div id="title2123" class="col-md-2">
                    <span id="Label32" data-type="SheetLabel" data-datafield="BP_SALESPERSON_ID" style="">销售人员</span>
                </div>
                <div id="Div45" class="col-md-4">
                    <select id="Control32" data-datafield="BP_SALESPERSON_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_sale_man" data-querycode="002" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
                <div id="title39" class="col-md-2">
                    <span id="Label47" data-type="SheetLabel" data-datafield="BUSINESS_PARTNER_ID" style="">经销商代码</span>
                </div>
                <div id="Div60" class="col-md-4">
                    <select id="Control47" data-datafield="BUSINESS_PARTNER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_business_partner_id" data-querycode="003" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="title63" class="col-md-2">
                    <span id="Label69" data-type="SheetLabel" data-datafield="STATE_CDE" style="">省</span>
                </div>
                <div id="Div86" class="col-md-4">
                    <select id="Control69" data-datafield="STATE_CDE" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_Province" data-querycode="004" data-datavaluefield="STATE_CDE" data-datatextfield="STATE_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
                <div id="title64" class="col-md-2">
                    <span id="Label70" data-type="SheetLabel" data-datafield="CITY_CDE" style="">城市</span>
                </div>
                <div id="Div87" class="col-md-4">
                    <select id="Control70" data-datafield="CITY_CDE" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_City" data-querycode="005" data-datavaluefield="CITY_CDE" data-datatextfield="CITY_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="Div12" class="col-md-2">
                    <span id="Span3" data-type="SheetLabel" data-datafield="VEHICLE_TYPE_CDE" style="">车辆类型</span>
                </div>
                <div id="Div13" class="col-md-4">
                    <select id="Text4" data-datafield="VEHICLE_TYPE_CDE" data-type="SheetDropDownList" style=""
                        data-schemacode="M_CarType" data-querycode="001" data-datavaluefield="VEHICLE_TYPE_CDE" data-datatextfield="VEHICLE_TYPE_DSC" data-selectedvalue="00001">
                    </select>
                </div>
                <div id="Div20" class="col-md-2">
                    <span id="Span7" data-type="SheetLabel" data-datafield="BP_DEALER_ID" style="">金融顾问</span>
                </div>

                <div id="Div21" class="col-md-4">
                    <select id="Text8" data-datafield="BP_DEALER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_Financial_Adviser" data-querycode="006" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="title26" class="col-md-2">
                    <span id="Label35" data-type="SheetLabel" data-datafield="CONTACT_PERSON" style="">联系方式</span>
                </div>
                <div id="Div48" class="col-md-4">
                    <input id="Control35" type="text" data-datafield="CONTACT_PERSON" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'30')">
                </div>
                <div id="title58" class="col-md-2">
                    <span id="Label65" data-type="SheetLabel" data-datafield="BP_SHOWROOM_ID" style="">展厅</span>
                </div>
                <div id="Div82" class="col-md-4">
                    <select id="Control65" data-datafield="BP_SHOWROOM_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_exhibition_hall" data-querycode="007" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="title49" class="col-md-2">
                    <span id="Label57" data-type="SheetLabel" data-datafield="REFERENCE_NBR" style="">申请参考号</span>
                </div>
                <div id="Div74" class="col-md-4">
                    <input id="Control57" type="text" data-datafield="REFERENCE_NBR" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'20')">
                </div>
            </div>

            <div class="row">
                <div id="title421" class="col-md-2">
                    <span id="Label379" data-type="SheetLabel" style="">风控审核结果</span>
                </div>
                <div id="Div3" class="col-md-4">
                    <input id="fkResult" data-datafield="fkResult" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row hidden">
                <div class="col-md-4">
                    <div id="title48101" class="leftbox">
                        <span>APPLICATION_TYPE_NAME</span>
                    </div>
                    
                    <div id="Div48101" class="rightbox">
                        <input id="Control48101" type="text" data-datafield="APPLICATION_TYPE_NAME" data-type="SheetTime" style="" class="">
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title48131" class="leftbox">
                        <span>APPLICATION_NAME</span>
                    </div>
                    
                    <div id="Div48131" class="rightbox">
                        <input id="Control48131" type="text" data-datafield="APPLICATION_NAME" data-type="SheetTime" style="" class="">
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title491" class="leftbox3">
                        <span id="Label571" data-type="SheetLabel" data-datafield="APPLICATION_DATE" style="">APPLICATION_DATE</span>
                    </div>
                    
                    <div id="Div741" class="rightbox3">
                        <input id="Control571" type="text" data-datafield="APPLICATION_DATE" data-type="SheetTime" style="" class="">
                    </div>
                </div>
            </div>
            <div class="row hidden">
                <div class="col-md-4">
                    <div class="leftbox">
                        贷款金额
                    </div>
                    
                    <div class="rightbox">
                        <input type="text" id="financedamount1" data-datafield="financedamount" data-type="SheetTextBox" style="" />
                    </div>
                </div>
            </div>
            <div class="row hidden">
                <div class="col-md-4">
                    <div class="leftbox">
                        bp_id
                    </div>
                    
                    <div class="rightbox">
                        <input type="text" id="hidden_001" data-datafield="bp_id" data-type="SheetTextBox" style="" />
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="leftbox">
                        USER_NAME
                    </div>
                    
                    <div class="rightbox">
                        <input type="text" id="hidden_002" data-datafield="USER_NAME" data-type="SheetTextBox" style="" data-defaultvalue="{Originator.LoginName}" />
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="leftbox">
                        PLAN_ID
                    </div>
                    
                    <div class="rightbox">
                        <input type="text" id="hidden_003" data-datafield="PLAN_ID" data-type="SheetTextBox" style="" />
                    </div>
                </div>
            </div>
            <div class="row hidden">
                <div class="col-md-12">
                    <div class="leftbox">
                        表单上下拉框数据源
                    </div>
                    
                    <div class="rightbox">
                        <textarea id="Control17" data-datafield="DDL_DATASOURCE" data-type="SheetRichTextBox" class="" style="">					</textarea>
                    </div>
                </div>
            </div>
            <div class="row hidden">
                <div class="col-md-12">
                    <div class="leftbox">
                        常用字段
                    </div>
                    
                    <div class="rightbox">
                        <textarea id="Textarea1" data-datafield="cyzd" data-type="SheetRichTextBox" class="" style="">					</textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="divContent" id="div_Detail_Individual" style="padding-right: 0%">
            <div class="row tableContent" id="detail_Individual">
                <div id="div695471" class="col-md-12">
                    <div id="ctl604410" data-datafield="APPLICANT_DETAIL" data-type="SheetDetail" class="" style="" data-filter="{IDENTIFICATION_CODE2}:{IDENTIFICATION_ID}"
                        data-defaultrowcount="0" data-displayadd="false">

                        <ul id="myTab_ctl604410" class="nav nav-tabs hidden">
                        </ul>
                        <div id="myTabContent_ctl604410" class="tab-content">
                            <div class="template">
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title49" class="col-md-4">
                                            <span id="ctl604410_label49" style="">姓名（中文）</span>
                                        </div>
                                        <div id="ctl604410_Data49" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.FIRST_THI_NME" style="" data-type="SheetTextBox" id="ctl604410_control49"
                                                data-onchange="setApplicationTypeName(this,'I')">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title43" class="col-md-4">
                                            <span id="ctl604410_label43" style="">证件类型</span>
                                        </div>
                                        <div id="ctl604410_Data43" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.ID_CARD_TYP" data-type="SheetDropDownList" id="ctl604410_control43" style=""
                                                data-masterdatacategory="证件类型" data-queryable="false" data-onchange="IdentifyCodeChange(this)">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title44" class="col-md-4">
                                            <span id="ctl604410_label44" style="">证件号码</span>
                                        </div>
                                        <div id="ctl604410_Data44" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.ID_CARD_NBR" data-type="SheetTextBox" id="ctl604410_control44" style="" data-onchange="IdentifyCodeChange(this)" onkeyup="this.value=strMaxLength(this.value,'40')">
                                        </div>
                                    </div>
                                </div>

                                <%--<div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title46" class="col-md-4">
                                            <span id="ctl604410_label46" style="">证件到期日</span>
                                        </div>
                                        <div id="ctl604410_Data46" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.ID_CARDEXPIRY_DTE" data-type="SheetTime" id="ctl604410_control46" style="" data-defaultvalue=""
                                                data-onchange="id_card_date_chg(this)">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title24" class="col-md-4">
                                            <span id="ctl604410_label24" style="">性别</span>
                                        </div>
                                        <div id="ctl604410_Data24" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.SEX" data-type="SheetDropDownList" id="ctl604410_control24" style=""
                                                data-masterdatacategory="性别" data-queryable="false" data-onchange="sex_chg(this)">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title62" class="col-md-4">
                                            <span id="ctl604410_label62" style="">户口所在地</span>
                                        </div>
                                        <div id="ctl604410_Data62" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.HUKOU_CDE" data-type="SheetDropDownList" id="ctl604410_control62" style=""
                                                data-masterdatacategory="户口所在地" data-queryable="false" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title15" class="col-md-4">
                                            <span id="ctl604410_label15" style="">出生日期</span>
                                        </div>
                                        <div id="ctl604410_Data15" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.DATE_OF_BIRTH" data-type="SheetTime" id="ctl604410_control15" style=""
                                                data-defaultvalue="" data-onchange="birthdayChange(this)" data-maxvalue="CurrentTime" data-minvalue="1900-01-01">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title78" class="col-md-4">
                                            <span id="ctl604410_label78" style="">民族</span>
                                        </div>
                                        <div id="ctl604410_Data78" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.NATION_CDE" data-type="SheetDropDownList" id="ctl604410_control78" style=""
                                                data-masterdatacategory="民族" data-queryable="false">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title79" class="col-md-4">
                                            <span id="ctl604410_label79" style="">签发机关</span>
                                        </div>
                                        <div id="ctl604410_Data79" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.ISSUING_AUTHORITY" style="" data-type="SheetTextBox" id="ctl604410_control79" onkeyup="this.value=strMaxLength(this.value,'500')">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title52" class="col-md-4">
                                            <span id="ctl604410_label52" style="">关系</span>
                                        </div>
                                        <div id="ctl604410_Data52" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE" data-type="SheetDropDownList" id="ctl604410_control52" style="" data-displayemptyitem="true"
                                                data-schemacode="M_relationship" data-querycode="fun_M_relationship" data-datavaluefield="RELATIONSHIP_CDE" data-datatextfield="RELATIONSHIP_DSC" data-onchange="relation_chg(this)">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Data76" class="col-md-4">
                                            <input type="checkbox" data-datafield="APPLICANT_DETAIL.SPOUSE_IND" data-type="SheetCheckbox" id="ctl604410_control76" style="" data-text="Spouse" data-onchange="spouse_chg(this)">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title75" class="col-md-4">
                                            <span id="ctl604410_label75" style="">驾照状态</span>
                                        </div>
                                        <div id="ctl604410_Data75" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.DRIVING_LICENSE_CODE" data-type="SheetDropDownList" id="ctl604410_control75" style=""
                                                data-masterdatacategory="驾照状态" data-queryable="false" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title25" class="col-md-4">
                                            <span id="ctl604410_label25" style="">邮箱地址</span>
                                        </div>
                                        <div id="ctl604410_Data25" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.EMAIL_ADDRESS" data-type="SheetTextBox" id="ctl604410_control25" style="" onkeyup="this.value=strMaxLength(this.value,'50')" 
                                                data-regularexpression="/^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/" data-regularinvalidtext="请输入有效的邮箱地址">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title53" class="col-md-4">
                                            <span id="ctl604410_label53" style="">教育程度</span>
                                        </div>
                                        <div id="ctl604410_Data53" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.EDUCATION_CDE" data-type="SheetDropDownList" id="ctl604410_control53" style=""
                                                data-masterdatacategory="教育程度" data-queryable="false" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title7" class="col-md-4">
                                            <span id="ctl604410_label7" style="">雇员类型</span>
                                        </div>
                                        <div id="ctl604410_Data7" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.EMPLOYMENT_TYPE_CDE" data-type="SheetDropDownList" id="ctl604410_control7" style=""
                                                data-masterdatacategory="雇员类型" data-queryable="false" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title4" class="col-md-4">
                                            <span id="ctl604410_label4" style="">行业类型</span>
                                        </div>
                                        <div id="ctl604410_Data4" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.INDUSTRY_TYPE_CDE" data-type="SheetDropDownList" id="ctl604410_control4" style="" data-displayemptyitem="true"
                                                data-schemacode="M_industry_type_code" data-querycode="009" data-datavaluefield="INDUSTRY_TYPE_CDE" data-datatextfield="INDUSTRY_TYPE_DSC">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title6" class="col-md-4">
                                            <span id="ctl604410_label6" style="">行业子类型</span>
                                        </div>
                                        <div id="ctl604410_Data6" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.INDUSTRY_SUBTYPE_CDE" data-type="SheetDropDownList" id="ctl604410_control6" style="" data-displayemptyitem="true"
                                                data-schemacode="M_industry_sub_type_cde" data-querycode="010" data-filter="APPLICANT_DETAIL.INDUSTRY_TYPE_CDE:industry_type_cde" data-datavaluefield="INDUSTRY_SUBTYPE_CDE" data-datatextfield="INDUSTRY_SUBTYPE_DSC">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title10" class="col-md-4">
                                            <span id="ctl604410_label10" style="">职业类型</span>
                                        </div>
                                        <div id="ctl604410_Data10" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.OCCUPATION_CDE" data-type="SheetDropDownList" id="ctl604410_control10" style="" data-displayemptyitem="true"
                                                data-schemacode="M_occupation_cde" data-querycode="011" data-datavaluefield="OCCUPATION_CDE" data-datatextfield="OCCUPATION_DSC">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title69" class="col-md-4">
                                            <span id="ctl604410_label69" style="">职业子类型</span>
                                        </div>
                                        <div id="ctl604410_Data69" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.SUB_OCCUPATION_CDE" data-type="SheetDropDownList" id="ctl604410_control69" style="" data-queryable="false"
                                                data-schemacode="M_sub_occupation" data-querycode="012" data-filter="APPLICANT_DETAIL.OCCUPATION_CDE:OCCUPATION_CDE" data-datavaluefield="SUB_OCCUPATION_CDE" data-datatextfield="SUB_OCCUPATION_DSC">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title70" class="col-md-4">
                                            <span id="ctl604410_label70" style="">职位</span>
                                        </div>
                                        <div id="ctl604410_Data70" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.DESIGNATION_CDE" data-type="SheetDropDownList" id="ctl604410_control70" style="" data-masterdatacategory="职位" data-queryable="false" data-displayemptyitem="true"></select>
                                            
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title71" class="col-md-4">
                                            <span id="ctl604410_label71" style="">工作组</span>
                                        </div>
                                        <div id="ctl604410_Data71" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.JOB_GROUP_CDE" data-type="SheetDropDownList" id="ctl604410_control71" style="" data-queryable="false" data-displayemptyitem="true"
                                                data-schemacode="M_job_group" data-querycode="014" data-filter="APPLICANT_DETAIL.DESIGNATION_CDE:designation_cde" data-datavaluefield="JOB_GROUP_CDE" data-datatextfield="JOB_GROUP_DSC">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title73" class="col-md-4">
                                            <span id="ctl604410_label73" style="">月收入</span>
                                        </div>
                                        <div id="ctl604410_Data73" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.ACTUAL_SALARY" data-type="SheetTextBox" id="ctl604410_control73" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title9" class="col-md-4">
                                            <span id="ctl604410_label9" style="">婚姻状况</span>
                                        </div>
                                        <div id="ctl604410_Data9" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.MARITAL_STATUS_CDE" data-type="SheetDropDownList" id="ctl604410_control9" style=""
                                                data-masterdatacategory="婚姻状况" data-queryable="false" data-displayemptyitem="true" data-onchange="marital_chg(this)">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title14" class="col-md-4">
                                            <span id="ctl604410_label14" style="">驾照号码</span>
                                        </div>
                                        <div id="ctl604410_Data14" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.LICENSE_NUMBER" data-type="SheetTextBox" id="ctl604410_control14" style="" 
                                                data-onchange="licenseNumberCheck(this)" onkeyup="this.value=strMaxLength(this.value,'18')"/>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title16" class="col-md-4">
                                            <span id="ctl604410_label16" style="">驾照到期日</span>
                                        </div>
                                        <div id="ctl604410_Data16" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.LICENSE_EXPIRY_DATE" data-type="SheetTime" id="ctl604410_control16" style=""
                                                data-defaultvalue="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title3" class="col-md-4">
                                            <span id="ctl604410_label3" style="">国籍</span>
                                        </div>
                                        <div id="ctl604410_Data3" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.RACE_CDE" data-type="SheetDropDownList" id="ctl604410_control3" style=""
                                                data-masterdatacategory="国籍" data-queryable="false">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title13" class="col-md-4">
                                            <span id="ctl604410_label13" style="">姓（英文）</span>
                                        </div>
                                        <div id="ctl604410_Data13" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.LAST_NAME" data-type="SheetTextBox" id="ctl604410_control13" style="" onkeyup="this.value=strMaxLength(this.value,'30')">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title11" class="col-md-4">
                                            <span id="ctl604410_label11" style="">名（英文）</span>
                                        </div>
                                        <div id="ctl604410_Data11" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.FIRST_NAME" style="" data-type="SheetTextBox" id="ctl604410_control11">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title12" class="col-md-4">
                                            <span id="ctl604410_label12" style="">中间名字</span>
                                        </div>
                                        <div id="ctl604410_Data12" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.MIDDLE_NAME" style="" data-type="SheetTextBox" id="ctl604410_control12">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title77" class="col-md-4">
                                            <span id="ctl604410_label77" style="">曾用名</span>
                                        </div>
                                        <div id="ctl604410_Data77" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.FORMER_NAME" style="" data-type="SheetTextBox" id="ctl604410_control77">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title72" class="col-md-4">
                                            <span id="ctl604410_label72" style="">预估收入</span>
                                        </div>
                                        <div id="ctl604410_Data72" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.SALARY_RANGE_CDE" data-type="SheetDropDownList" id="ctl604410_control72" style="" data-queryable="false" data-displayemptyitem="true"
                                                data-schemacode="M_salary_range" data-querycode="fun_M_salary_range" data-datavaluefield="SALARY_RANGE_CDE" data-datatextfield="SALARY_RANGE_DSC">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title45" class="col-md-4">
                                            <span id="ctl604410_label45" style="">证件发行日</span>
                                        </div>
                                        <div id="ctl604410_Data45" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.ID_CARDISSUE_DTE" data-type="SheetTime" id="ctl604410_control45" style=""
                                                data-defaultvalue="" data-onchange="id_card_date_chg(this)">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title18" class="col-md-4">
                                            <span id="ctl604410_label18" style="">年龄</span>
                                        </div>
                                        <div id="ctl604410_Data18" class="col-md-4">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.AGE_IN_YEAR" data-type="SheetTextBox" id="ctl604410_control18" readonly style="width: 50%" >年
                                        </div>
                                        <div id="ctl604410_Data19" class="col-md-4">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.AGE_IN_MONTH" data-type="SheetTextBox" id="ctl604410_control19" readonly style="width: 50%" s>月
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title68" class="col-md-4">
                                            <span id="ctl604410_label68" style="">房产所有人</span>
                                        </div>
                                        <div id="ctl604410_Data68" class="col-md-8">
                                            <select data-datafield="APPLICANT_DETAIL.HOUSE_OWNER_CDE" data-type="SheetDropDownList" id="ctl604410_control68" style="" data-queryable="false" data-displayemptyitem="true"
                                                data-schemacode="M_house_owner_code" data-querycode="fun_M_house_owner_code" data-datavaluefield="HOUSE_OWNER_CDE" data-datatextfield="HOUSE_OWNER_DSC">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title37" class="col-md-4">
                                            <span id="ctl604410_label37" style="">子女</span>
                                        </div>
                                        <div id="ctl604410_Data37" class="col-md-8">
                                            <input type="checkbox" data-datafield="APPLICANT_DETAIL.CHILDREN_FLAG" data-type="SheetCheckbox" id="ctl604410_control37" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title63" class="col-md-4">
                                            <span id="ctl604410_label63" style="">家庭人数</span>
                                        </div>
                                        <div id="ctl604410_Data63" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.NO_OF_FAMILY_MEMBERS" data-type="SheetTextBox" id="ctl604410_control63" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl604410_Title17" class="col-md-4">
                                            <span id="ctl604410_label17" style="">供养人数</span>
                                        </div>
                                        <div id="ctl604410_Data17" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.NUMBER_OF_DEPENDENT" data-type="SheetTextBox" id="ctl604410_control17" style=""
                                                onkeyup="this.value=strMaxLength(this.value,'10')">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl604410_Title1" class="col-md-4">
                                            <span id="ctl604410_label1" style="">IDENTIFICATION_CODE</span>
                                        </div>
                                        <div id="ctl604410_Data1" class="col-md-8">
                                            <input type="text" data-datafield="APPLICANT_DETAIL.IDENTIFICATION_CODE2" data-type="SheetTextBox" id="ctl604410_control1" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
										<div id="ctl604410_Title8" class="col-md-4">
											<span id="ctl604410_label8" style="">头衔</span>
										</div>
										<div id="ctl604410_Data8" class="col-md-8">
											<input type="text" data-datafield="APPLICANT_DETAIL.TITLE_CDE" data-type="SheetTextBox" id="ctl604410_control8" style=""
                                                data-defaultvalue="00001">
 										</div>
									</div>
                                    <div class="col-md-4 hidden">
										<div id="ctl604410_Title57" class="col-md-4">
											<span id="ctl604410_label57" style="">头衔（英文）</span>
										</div>
										<div id="ctl604410_Data57" class="col-md-8">
											<input type="text" data-datafield="APPLICANT_DETAIL.THAI_TITLE_CDE" data-type="SheetTextBox" id="ctl604410_control57" style=""
                                                data-defaultvalue="00001">
 										</div>
									</div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="col-md-4">
                                            <span style="">同意</span>
                                        </div>
                                        <div class="col-md-8">
                                            <input type="checkbox" data-datafield="APPLICANT_DETAIL.PRIVACY_ACT" data-type="SheetCheckbox" id="ctl604410_control20" style="" data-text="同意">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="col-md-4">
                                            <span style="">贵宾</span>
                                        </div>
                                        <div class="col-md-8">
                                            <input type="checkbox" data-datafield="APPLICANT_DETAIL.VIP_IND" data-type="SheetCheckbox" id="ctl604410_control67" style="" data-text="贵宾">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div  class="col-md-4">
                                            <span style="">工作人员</span>
                                        </div>
                                        <div class="col-md-8">
                                           <input type="checkbox" data-datafield="APPLICANT_DETAIL.STAFF_IND" data-type="SheetCheckbox" id="ctl604410_control66" style="" data-text="工作人员">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="col-md-4">
                                            <span style="">黑名单中无记录</span>
                                        </div>
                                        <div class="col-md-8">
                                            <input type="checkbox" data-datafield="APPLICANT_DETAIL.BLACKLIST_NORECORD_IND" data-type="SheetCheckbox" id="ctl604410_control55" style="" data-text="黑名单中无记录"
                                                    disabled data-defaultvalue="true">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="col-md-4">
                                            <span style="">外部信用记录</span>
                                        </div>
                                        <div class="col-md-8">
                                            <input type="checkbox" data-datafield="APPLICANT_DETAIL.BLACKLIST_IND" data-type="SheetCheckbox" id="ctl604410_control48" style="" data-text="外部信用记录"
                                                    disabled>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div  class="col-md-4">
                                            <span style="">抵押人</span>
                                        </div>
                                        <div class="col-md-8">
                                           <input type="checkbox" data-datafield="APPLICANT_DETAIL.LIENEE" data-type="SheetCheckbox" id="ctl604410_control74" style="" data-text="抵押人"
                                                    data-onchange="ck_Lienee_change(this)">
                                        </div>
                                    </div>
                                </div>--%>
                                
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div class="divContent" id="div_Detail_Company" style="padding-right: 0%">
            <div class="row tableContent" id="detail_Company">
                <div id="div322965" class="col-md-12">
                    <div id="ctl863190" data-datafield="COMPANY_DETAIL" data-type="SheetDetail" class="" style="" data-filter="{IDENTIFICATION_CODE3}:{IDENTIFICATION_ID}"
                        data-defaultrowcount="0" data-displayadd="false">

                        <ul id="myTab_ctl863190" class="nav nav-tabs hidden">
                        </ul>
                        <div id="myTabContent_ctl863190" class="tab-content">
                            <div class="template">
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl863190_Title1" class="col-md-4">
                                            <span id="ctl863190_label1" style="">IDENTIFICATION_CODE</span>
                                        </div>
                                        <div id="ctl863190_Data1" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.IDENTIFICATION_CODE3" data-type="SheetTextBox" id="ctl863190_control1" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title34" class="col-md-4">
                                            <span id="ctl863190_label34" style="">组织机构代码</span>
                                        </div>
                                        <div id="ctl863190_Data34" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.ORGANIZATION_CDE" data-type="SheetTextBox" id="ctl863190_control34" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title25" class="col-md-4">
                                            <span id="ctl863190_label25" style="">注册号</span>
                                        </div>
                                        <div id="ctl863190_Data25" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.COMPANY_STS" data-type="SheetTextBox" id="ctl863190_control25" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title26" class="col-md-4">
                                            <span id="ctl863190_label26" style="">公司名称（中文）</span>
                                        </div>
                                        <div id="ctl863190_Data26" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.COMPANY_THI_NME" style="" data-type="SheetTextBox" id="ctl863190_control26">
                                        </div>
                                    </div>
                                </div>

                                <%--<div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title2" class="col-md-4">
                                            <span id="ctl863190_label2" style="">企业类型</span>
                                        </div>
                                        <div id="ctl863190_Data2" class="col-md-8">
                                            <select data-datafield="COMPANY_DETAIL.BUSINESS_TYPE_CDE" data-type="SheetDropDownList" id="ctl863190_control2" style="" data-displayemptyitem="true" data-queryable="false"
                                                data-schemacode="M_business_type" data-querycode="fun_M_business_type" data-datavaluefield="BUSINESS_TYPE_CDE" data-datatextfield="BUSINESS_TYPE_DSC">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title37" class="col-md-4">
                                            <span id="ctl863190_label37" style="">注册资本金额</span>
                                        </div>
                                        <div id="ctl863190_Data37" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.CAPITAL_REGISTRATION_AMT" data-type="SheetTextBox" id="ctl863190_control37" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title31" class="col-md-4">
                                            <span id="ctl863190_label31" style="">成立自</span>
                                        </div>
                                        <div id="ctl863190_Data31" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.ESTABLISHED_SINCE" data-type="SheetTime" id="ctl863190_control31" style="" data-defaultvalue="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title11" class="col-md-4">
                                            <span id="ctl863190_label11" style="">母公司</span>
                                        </div>
                                        <div id="ctl863190_Data11" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.PARENT_COMPANY" style="" data-type="SheetTextBox" id="ctl863190_control11">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title12" class="col-md-4">
                                            <span id="ctl863190_label12" style="">子公司</span>
                                        </div>
                                        <div id="ctl863190_Data12" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.SUBSIDIARY_COMPANY" style="" data-type="SheetTextBox" id="ctl863190_control12">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title3" class="col-md-4">
                                            <span id="ctl863190_label3" style="">行业类型</span>
                                        </div>
                                        <div id="ctl863190_Data3" class="col-md-8">
                                            <select data-datafield="COMPANY_DETAIL.INDUSTRY_TYPE_CDE" data-type="SheetDropDownList" id="ctl863190_control3" style="" data-displayemptyitem="true"
                                                data-schemacode="M_industry_type_code" data-querycode="009" data-datavaluefield="INDUSTRY_TYPE_CDE" data-datatextfield="INDUSTRY_TYPE_DSC">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title4" class="col-md-4">
                                            <span id="ctl863190_label4" style="">行业子类型</span>
                                        </div>
                                        <div id="ctl863190_Data4" class="col-md-8">
                                            <select data-datafield="COMPANY_DETAIL.INDUSTRY_SUBTYPE_CDE" data-type="SheetDropDownList" id="ctl863190_control4" style=""
                                                data-schemacode="M_industry_sub_type_cde" data-querycode="010" data-filter="COMPANY_DETAIL.INDUSTRY_TYPE_CDE:industry_type_cde" data-datavaluefield="INDUSTRY_SUBTYPE_CDE" data-datatextfield="INDUSTRY_SUBTYPE_DSC">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title39" class="col-md-4">
                                            <span id="ctl863190_label39" style="">法人姓名</span>
                                        </div>
                                        <div id="ctl863190_Data39" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.REP_NAME" style="" data-type="SheetTextBox" id="ctl863190_control39">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title42" class="col-md-4">
                                            <span id="ctl863190_label42" style="">法人身份证件号码</span>
                                        </div>
                                        <div id="ctl863190_Data42" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.REP_ID_CARD_NO" data-type="SheetTextBox" id="ctl863190_control42" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title44" class="col-md-4">
                                            <span id="ctl863190_label44" style="">法人职务</span>
                                        </div>
                                        <div id="ctl863190_Data44" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.REPRESENTATIVE_DESIGNATION" style="" data-type="SheetTextBox" id="ctl863190_control44">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title40" class="col-md-4">
                                            <span id="ctl863190_label40" style="">贷款卡号</span>
                                        </div>
                                        <div id="ctl863190_Data40" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.LENDING_CODE" data-type="SheetTextBox" id="ctl863190_control40" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title43" class="col-md-4">
                                            <span id="ctl863190_label43" style="">贷款卡密码</span>
                                        </div>
                                        <div id="ctl863190_Data43" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.LOAN_CRD_PWD" data-type="SheetTextBox" id="ctl863190_control43" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title5" class="col-md-4">
                                            <span id="ctl863190_label5" style="">公司名称（英文）</span>
                                        </div>
                                        <div id="ctl863190_Data5" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.NAME3" style="" data-type="SheetTextBox" id="ctl863190_control5">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title14" class="col-md-4">
                                            <span id="ctl863190_label14" style="">公司年限</span>
                                        </div>
                                        <div id="ctl863190_Data14" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.BUSINESS_HISTORY" style="" data-type="SheetTextBox" id="ctl863190_control14">
                                        </div>
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title15" class="col-md-4">
                                            <span id="ctl863190_label15" style="">信托机构名称</span>
                                        </div>
                                        <div id="ctl863190_Data15" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.FUTURE_DIRECTION" style="" data-type="SheetTextBox" id="ctl863190_control15">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title13" class="col-md-4">
                                            <span id="ctl863190_label13" style="">收入</span>
                                        </div>
                                        <div id="ctl863190_Data13" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.FLEET_SIZE" data-type="SheetTextBox" id="ctl863190_control13" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title36" class="col-md-4">
                                            <span id="ctl863190_label36" style="">净资产额</span>
                                        </div>
                                        <div id="ctl863190_Data36" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.NET_WORTH_AMT" data-type="SheetTextBox" id="ctl863190_control36" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl863190_Title17" class="col-md-4">
                                            <span id="ctl863190_label17" style="">邮箱地址</span>
                                        </div>
                                        <div id="ctl863190_Data17" class="col-md-8">
                                            <input type="text" data-datafield="COMPANY_DETAIL.EMAIL_ADDRESS" data-type="SheetTextBox" id="ctl863190_control17" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div id="ctl863190_Data30" class="col-md-2">
                                            <input type="checkbox" data-datafield="COMPANY_DETAIL.BLACKLIST_NORECORD_IND" data-type="SheetCheckbox" id="ctl863190_control30" style="" disabled data-defaultvalue="true" data-text="黑名单中无记录">
                                        </div>
                                        <div id="ctl863190_Data28" class="col-md-2">
                                            <input type="checkbox" data-datafield="COMPANY_DETAIL.BLACKLIST_IND" data-type="SheetCheckbox" id="ctl863190_control28" style="" disabled data-text="外部信用記錄">
                                        </div>
                                        <div id="ctl863190_Data45" class="col-md-2">
                                            <input type="checkbox" data-datafield="COMPANY_DETAIL.LIENEE" data-type="SheetCheckbox" id="ctl863190_control45" style="" data-text="抵押人">
                                        </div>

                                    </div>
                                </div>
                                <div class="row bottom">
                                    <div class="col-md-12">
                                        <div id="ctl863190_Title16" class="col-md-2" style="width: 11.11%">
                                            <span id="ctl863190_label16" style="">评论</span>
                                        </div>
                                        <div id="ctl863190_Data16" class="col-md-10" style="width: 88%">
                                            <input type="text" data-datafield="COMPANY_DETAIL.COMPANY_DETAIL_COMMENT" style="" data-type="SheetTextBox" id="ctl863190_control16">											</input>
                                        </div>
                                    </div>
                                </div>--%>

                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        <div class="divContent" id="div_Detail_VEHICLE" style="padding-right: 0%">
            <div class="row tableContent">
                <div id="div665779" class="col-md-12">
                    <div id="ctl524044" data-datafield="VEHICLE_DETAIL" data-type="SheetDetail" class="" style=""
                        data-defaultrowcount="1" data-displayadd="false">
                        <ul id="myTab_ctl524044" class="nav nav-tabs hidden">
                        </ul>
                        <div id="myTabContent_ctl524044" class="tab-content">
                            <div class="template">
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title9" class="col-md-4">
                                            <span id="ctl524044_label9" style="">资产状况</span>
                                        </div>
                                        <div id="ctl524044_Data9" class="col-md-8">
                                            <select data-datafield="VEHICLE_DETAIL.CONDITION" data-type="SheetDropDownList" id="ctl524044_control9" style=""
                                                data-masterdatacategory="资产状况" data-queryable="false" data-onchange="asset_Condition_Change(this)">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title7" class="col-md-4">
                                            <span id="ctl524044_label7" style="">购车目的</span>
                                        </div>
                                        <div id="ctl524044_Data7" class="col-md-8">
                                            <select data-datafield="VEHICLE_DETAIL.USAGE7" data-type="SheetDropDownList" id="ctl524044_control7" style=""
                                                data-masterdatacategory="购车目的" data-queryable="false">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title3" class="col-md-4">
                                            <span id="ctl524044_label3" style="">制造商</span>
                                        </div>
                                        <div id="ctl524044_Data3" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MAKE_DSC" disabled data-type="SheetTextBox" id="ctl524044_control3" style="width: 80%"
                                                data-schemacode="M_VEHICLE" data-querycode="fun_M_VEHICLE" data-popupwindow="PopupWindow"
                                                data-outputmappings="VEHICLE_DETAIL.ASSET_MAKE_CDE:asset_make_cde,VEHICLE_DETAIL.ASSET_MAKE_DSC:asset_make_dsc,VEHICLE_DETAIL.ASSET_BRAND_CDE:asset_brand_cde,VEHICLE_DETAIL.ASSET_BRAND_DSC:asset_brand_dsc,VEHICLE_DETAIL.ASSET_MODEL_CDE:asset_model_cde,VEHICLE_DETAIL.COMMENTS7:asset_model_dsc,VEHICLE_DETAIL.POWER_PARAMETER:asset_model_dsc,VEHICLE_DETAIL.MIOCN_ID:miocn_id,VEHICLE_DETAIL.MIOCN_NBR:miocn_nbr,VEHICLE_DETAIL.MIOCN_DSC:miocn_dsc,VEHICLE_DETAIL.NEW_PRICE:retail_price_amt,CONTRACT_DETAIL.TOTAL_ASSET_COST:retail_price_amt,VEHICLE_DETAIL.VEHICLE_TYPE_CDE:vehicle_type_cde,VEHICLE_DETAIL.VEHICLE_SUBTYPE_CDE:vehicle_subtyp_cde,VEHICLE_DETAIL.TRANSMISSION:transmission_type_cde">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title75" class="col-md-4">
                                            <span id="ctl524044_label75" style="">车型</span>
                                        </div>
                                        <div id="ctl524044_Data75" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_BRAND_DSC" disabled data-type="SheetTextBox" id="ctl524044_control75" style="">
                                        </div>
                                    </div>

                                    <div class="col-md-4">
                                        <div id="ctl524044_Title73" class="col-md-4">
                                            <span id="ctl524044_label73" style="">动力参数</span>
                                        </div>
                                        <div id="ctl524044_Data73" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.POWER_PARAMETER" disabled style="" data-type="SheetTextBox" id="ctl524044_control73"
                                                data-onchange="power_parameter_chg(this)" />
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title79" class="col-md-4">
                                            <span id="ctl524044_label79" style="">资产编码</span>
                                        </div>
                                        <div id="ctl524044_Data79" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_NBR" readonly data-type="SheetTextBox" id="ctl524044_control79" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row bottom">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title24" class="col-md-4">
                                            <span id="ctl524044_label24" style="">资产价格</span>
                                        </div>
                                        <div id="ctl524044_Data24" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.NEW_PRICE" readonly data-type="SheetTextBox" id="ctl524044_control24" style="" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <%--<div class="col-md-4">
                                        <div id="ctl524044_Title19" class="col-md-4">
                                            <span id="ctl524044_label19" style="">发动机号码</span>
                                        </div>
                                        <div id="ctl524044_Data19" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ENGINE" data-type="SheetTextBox" id="ctl524044_control19"
                                                style="" onkeyup="this.value=strMaxLength(this.value,'40')">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title28" class="col-md-4">
                                            <span id="ctl524044_label28" style="">VIN码</span>
                                        </div>
                                        <div id="ctl524044_Data28" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.VIN_NUMBER" data-type="SheetTextBox" maxlength="17" id="ctl524044_control28"
                                                style="" data-onchange="vin_change(this)" onkeyup="this.value=strMaxLength(this.value,'25')">
                                        </div>
                                    </div>--%>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524045_Title3" class="col-md-4">
                                            <span id="ctl524045_label3" style="">制造商Code</span>
                                        </div>
                                        <div id="ctl524045_Data3" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MAKE_CDE" data-type="SheetTextBox" id="ctl524045_control3" style="" />
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524045_Title75" class="col-md-4">
                                            <span id="ctl524045_label75" style="">车型Code</span>
                                        </div>
                                        <div id="ctl524045_Data75" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_BRAND_CDE" data-type="SheetTextBox" id="ctl524045_control75" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title71" class="col-md-4">
                                            <span id="ctl524044_label71" style="">VEHICLE_TYPE_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data71" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_TYPE_CDE" data-type="SheetTextBox" id="ctl524044_control71" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title72" class="col-md-4">
                                            <span id="ctl524044_label72" style="">VEHICLE_SUBTYPE_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data72" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_SUBTYPE_CDE" data-type="SheetTextBox" id="ctl524044_control72" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title4" class="col-md-4">
                                            <span id="ctl524044_label4" data-type="SheetLabel" style="">ASSET_MODEL_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data4" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MODEL_CDE" data-type="SheetTextBox" id="ctl524044_control4" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title78" class="col-md-4">
                                            <span id="ctl524044_label78" style="">资产ID</span>
                                        </div>
                                        <div id="ctl524044_Data78" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_ID" data-type="SheetTextBox" id="ctl524044_control78" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title80" class="col-md-4">
                                            <span id="ctl524044_label80" style="">MIOCN_DSC</span>
                                        </div>
                                        <div id="ctl524044_Data80" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_DSC" data-type="SheetTextBox" id="ctl524044_control80" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl5240414_Title83" class="col-md-4">
                                            <span id="ctl5240414_label83" style="">担保方式</span>
                                        </div>
                                        <div id="ctl5240414_Data83" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.GURANTEE_OPTION" data-type="SheetTextBox" id="ctl8441175" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-12 hidden">
                                        <div id="ctl5245044_Title45" class="col-md-4" style="width: 11.11%">
                                            <span id="ctl5245044_label45" style="">备注</span>
                                        </div>
                                        <div id="ctl5245044_Data45" class="col-md-8" style="width: 88%">
                                            <textarea data-datafield="VEHICLE_DETAIL.VEHICLE_COMMENT" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl5245044_control45" onkeyup="this.value=strMaxLength(this.value,'255')"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-2">
                    <span data-type="SheetLabel" data-datafield="Bankname" style="">发动机号码</span>
                </div>
                <div class="col-md-4">
                    <input type="text" data-datafield="engine_number" data-type="SheetTextBox" id="ctl_engine_number" class="" style=""
                        onkeyup="this.value=strMaxLength(this.value,'40')" />
                </div>
                <div class="col-md-2">
                    <span data-type="SheetLabel" data-datafield="Branchname" style="">VIN码</span>
                </div>
                <div class="col-md-4">
                    <input type="text" data-datafield="vin_number" data-type="SheetTextBox" id="ctl_vin_number" class="" style=""
                        data-onchange="vin_change(this)" maxlength="17" />
                </div>
            </div>
            <div class="row">
                <div id="title461" class="col-md-2">
                    <span id="Label413" data-type="SheetLabel" data-datafield="Bankname" style="">扣款银行</span>
                </div>
                <div id="control461" class="col-md-4">
                    <select data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" style="" data-displayemptyitem="true" data-schemacode="yhld" data-querycode="yhld"   data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
                </div>
                <div id="title462" class="col-md-2">
                    <span id="Label414" data-type="SheetLabel" data-datafield="branchname" style="">银行分支</span>
                </div>
                <div id="control462" class="col-md-4">

                    <select data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-displayemptyitem="true" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
                </div>
            </div>
            <div class="row">
                <div id="title463" class="col-md-2">
                    <span id="Label415" data-type="SheetLabel" data-datafield="Accoutname" style="">账户名</span>
                </div>
                <div id="control463" class="col-md-4">
                    <select id="Control415s" style="display: none;" onchange="chkzhm(this)"></select>
                    <input id="Control415" type="text" data-datafield="Accoutname" data-type="SheetTextBox" style="">
                    <button type="button" id="btn_update_to_cap" onclick="update_to_cap('true')" class="button button-small button-positive" style="line-height: 20px;margin:0 120px">更新到CAP</button>
                </div>
                <div id="title464" class="col-md-2">
                    <span id="Label416" data-type="SheetLabel" data-datafield="AccoutNumber" style="">账户号</span>
                </div>
                <div id="control464" class="col-md-4">
                    <input id="Control416" type="text" oninput="textnum(this)" data-datafield="AccoutNumber" data-type="SheetTextBox" style="" maxlength="20">
                </div>
            </div>
            <div class="row tableContent">
                <div id="title185" class="col-md-2">
                    <span id="Label145" data-type="SheetLabel" data-datafield="ASSET_ACCESSORY" style="">附加费明细表</span>
                </div>
                <div id="control185" class="col-md-10">
                    <table id="Control145" data-datafield="ASSET_ACCESSORY" data-type="SheetGridView" class="SheetGridView">
                        <tbody>
                            <tr class="header">
                                <td id="Control145_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control145_Header3" data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE">
                                    <label id="Control145_Label3" data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE" data-type="SheetLabel" style="">选装部件</label>
                                </td>
                                <td id="Control145_Header4" data-datafield="ASSET_ACCESSORY.PRICE" style="width: 180px">
                                    <label id="Control145_Label4" data-datafield="ASSET_ACCESSORY.PRICE" data-type="SheetLabel" style="">价格</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control145_Option" class="rowOption"></td>
                                <td data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE">
                                    <select id="Control145_ctl3" data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE" data-type="SheetDropDownList" style="width: 92%" data-displayemptyitem="true"
                                        data-schemacode="M_accessory_code" data-querycode="fun_M_accessory_code" data-datavaluefield="ACCESSORY_CDE" data-datatextfield="ACCESSORY_DSC">
                                    </select>
                                </td>
                                <td data-datafield="ASSET_ACCESSORY.PRICE">
                                    <input id="Control145_ctl4" type="text" data-datafield="ASSET_ACCESSORY.PRICE" data-type="SheetTextBox" style="width: 92%" data-formatrule="{0:F2}">
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
        </div>

        <%-- 金融条款表 --%>
        <div class="divContent" id="div_Detail_CONTRACT" style="padding-right: 0%">
            <div class="row tableContent">
                <div id="div505260" class="col-md-12">
                    <div id="ctl508406" data-datafield="CONTRACT_DETAIL" data-type="SheetDetail" class="" style=""
                        data-defaultrowcount="1" data-displayadd="false">
                        <ul id="myTab_ctl508406" class="nav nav-tabs hidden">
                        </ul>
                        <div id="myTabContent_ctl508406" class="tab-content">
                            <div class="template">
                                <div class="row">
                                    <div class="col-md-8">
                                        <div id="ctl508406_Title114" class="col-md-4" style="width: 16.67%">
                                            <span id="ctl508406_label114" style="">产品组</span>
                                        </div>
                                        <div id="ctl508406_Data114" class="col-md-8" style="width: 83%">
                                            <select data-datafield="CONTRACT_DETAIL.FP_GROUP_ID" data-type="SheetDropDownList" id="ctl508406_control114" style=""
                                                data-schemacode="M_financial_product_group" data-querycode="fun_M_financial_product_group" data-datavaluefield="FP_GROUP_ID" data-datatextfield="FP_GROUP_DSC" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title95" class="col-md-4">
                                            <span id="ctl508406_label95" style="">付款频率</span>
                                        </div>
                                        <div id="ctl508406_Data95" class="col-md-8">
                                            <select data-datafield="CONTRACT_DETAIL.FREQUENCY_CDE" data-type="SheetDropDownList" id="ctl508406_control95" style=""
                                                data-masterdatacategory="付款频率">
                                            </select>
                                        </div>
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-md-8">
                                        <div id="ctl508406_Title4" class="col-md-4" style="width: 16.67%">
                                            <span id="ctl508406_label4" style="">产品类型</span>
                                        </div>
                                        <div id="ctl508406_Data4" class="col-md-8" style="width: 83%">
                                            <select data-datafield="CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID" data-type="SheetDropDownList" id="ctl508406_control4" style=""
                                                data-schemacode="M_FinancialType" data-querycode="fun_M_FinancialType" data-datavaluefield="financial_product_id" data-datatextfield="financial_product_dsc" data-displayemptyitem="true"
                                                data-filter="VEHICLE_DETAIL.CONDITION:asset_condition,APPLICATION_TYPE_CODE:type,bp_id:bp_id,CONTRACT_DETAIL.FP_GROUP_ID:group_id,VEHICLE_DETAIL.ASSET_MODEL_CDE:model_cde">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508407_Title27" class="col-md-4">
                                            <span id="ctl508407_label27" style="">车辆销售价格</span>
                                        </div>
                                        <div id="ctl508407_Data27" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.TOTAL_ASSET_COST" data-type="SheetTextBox" id="ctl508407_control27" style=""
                                                data-onchange="fpp_chg(this,'sp')" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title27" class="col-md-4">
                                            <span id="ctl508406_label27" style="">贷款期数（月）</span>
                                        </div>
                                        <div id="ctl508406_Data27" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.LEASE_TERM_IN_MONTH" data-type="SheetTextBox" id="ctl508406_control27" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title159" class="col-md-4">
                                            <span id="ctl508406_label159" style="">销售价格</span>
                                        </div>
                                        <div id="ctl508406_Data159" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.SALE_PRICE" data-type="SheetTextBox" disabled id="ctl508406_control159" style=""
                                                data-computationrule="{CONTRACT_DETAIL.TOTAL_ASSET_COST}+{CONTRACT_DETAIL.ACCESSORY_AMT}" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title10" class="col-md-4">
                                            <span id="ctl508406_label10" style="">合同价格</span>
                                        </div>
                                        <div id="ctl508406_Data10" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.ASSET_COST" data-type="SheetTextBox" disabled id="ctl508406_control10" style=""
                                                data-computationrule="{CONTRACT_DETAIL.SALE_PRICE}" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title75" class="col-md-4">
                                            <span id="ctl508406_label75" style="">首付款比例%</span>
                                        </div>
                                        <div id="ctl508406_Data75" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT" data-type="SheetTextBox" id="ctl508406_control75" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title89" class="col-md-4">
                                            <span id="ctl508406_label89" style="">贷款额比例 %</span>
                                        </div>
                                        <div id="ctl508406_Data89" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.FINANCED_AMT_PCT" disabled data-type="SheetTextBox" id="ctl508406_control89" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title23" class="col-md-4">
                                            <span id="ctl508406_label23" style="">资产残值/轻松融资尾款%</span>
                                        </div>
                                        <div id="ctl508406_Data23" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.BALLOON_PERCENTAGE" data-type="SheetTextBox" disabled id="ctl508406_control23" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title14" class="col-md-4">
                                            <span id="ctl508406_label14" style="">首付款金额</span>
                                        </div>
                                        <div id="ctl508406_Data14" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.CASH_DEPOSIT" data-type="SheetTextBox" id="ctl508406_control14" style="" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title21" class="col-md-4">
                                            <span id="ctl508406_label21" style="">贷款金额</span>
                                        </div>
                                        <div id="ctl508406_Data21" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.AMOUNT_FINANCED" disabled data-type="SheetTextBox" id="ctl508406_control21" style=""
                                                data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title25" class="col-md-4">
                                            <span id="ctl508406_label25" style="">资产残值/轻松融资尾款金额</span>
                                        </div>
                                        <div id="ctl508406_Data25" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.BALLOON_AMOUNT" data-type="SheetTextBox" disabled id="ctl508406_control25" style="" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title124" class="col-md-4">
                                            <span id="ctl508406_label124" style="">客户利率%</span>
                                        </div>
                                        <div id="ctl508406_Data124" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.BASE_CUSTOMER_RATE" disabled data-type="SheetTextBox" id="ctl508406_control124" style="" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title141" class="col-md-4">
                                            <span id="ctl508406_label141" style="">实际利率%</span>
                                        </div>
                                        <div id="ctl508406_Data141" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.ACTUAL_RTE" disabled data-type="SheetTextBox" id="ctl508406_control141" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title139" class="col-md-4">
                                            <span id="ctl508406_label139" style="">贴息利率%</span>
                                        </div>
                                        <div id="ctl508406_Data139" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.CALC_SUBSIDY_RTE" disabled data-type="SheetTextBox" id="ctl508406_control139" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508407_Title124" class="col-md-4">
                                            <span id="ctl508407_label124" style="">应付总额</span>
                                        </div>
                                        <div id="ctl508407_Data124" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.YFJE" data-type="SheetTextBox" id="ctl508407_control124" style=""
                                                disabled data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title140" class="col-md-4">
                                            <span id="ctl508406_label140" style="">贴息金额</span>
                                        </div>
                                        <div id="ctl508406_Data140" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.CALC_SUBSIDY_AMT" disabled data-type="SheetTextBox" id="ctl508406_control140" style="" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title62" class="col-md-4">
                                            <span id="ctl508406_label62" style="">利息总额</span>
                                        </div>
                                        <div id="ctl508406_Data62" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.ASSETITC" disabled data-type="SheetTextBox" id="ctl508406_control62" style="" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl508406_Title1" class="col-md-4">
                                            <span id="ctl508406_label1" style="">IDENTIFICATION_CODE</span>
                                        </div>
                                        <div id="ctl508406_Data1" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.IDENTIFICATION_CODE8" data-type="SheetTextBox" id="ctl508406_control1" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl508407_Title126" class="col-md-4">
                                            <span id="ctl508407_label126" style="">总附加费价格</span>
                                        </div>
                                        <div id="ctl508407_Data126" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.TOTAL_ACCESSORY_AMT" data-type="SheetTextBox" id="ctl508407_control126" style=""
                                                data-computationrule="{CONTRACT_DETAIL.ACCESSORY_AMT}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row bottom">
                                    <div class="col-md-4">
                                        <div id="ctl508407_Title62" class="col-md-4">
                                            <span id="ctl508407_label62" style="">未偿余额</span>
                                        </div>
                                        <div id="ctl508407_Data62" class="col-md-8">
                                            <%--Netsol没有保存到数据库--%>
                                            <input type="text" data-datafield="CONTRACT_DETAIL.WCYE" data-type="SheetTextBox" id="ctl508407_control62" style=""
                                                disabled data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title157" class="col-md-4">
                                            <span id="ctl508406_label157" style="">展期期数</span>
                                        </div>
                                        <div id="ctl508406_Data157" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.DEFERRED_TRM" disabled data-type="SheetTextBox" id="ctl508406_control157" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title126" class="col-md-4">
                                            <span id="ctl508406_label126" style="">附加费价格</span>
                                        </div>
                                        <div id="ctl508406_Data126" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.ACCESSORY_AMT" disabled data-type="SheetTextBox" id="ctl508406_control126" style=""
                                                data-computationrule="SUM({ASSET_ACCESSORY.PRICE})" data-onchange="fpp_chg(this,'sp')" data-formatrule="{0:F2}">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" id="divattachmentinfo" onclick="hidediv('divattachment',this)">
            <label id="Label1" data-en_us="Sheet information">附件信息</label>
        </div>
        <div class="divContent" id="divattachment">
            <div id="divfjll">
                <ul id="ulfjll">
                </ul>
            </div>
            <div class="row" style="display: none">
                <div id="title81" class="col-md-2">
                    <span id="Label181" data-type="SheetLabel" data-datafield="WorkflowType" style="">流程类型</span>
                </div>
                <div id="contro8l1" class="col-md-4">
                    <input id="Control117" type="text" data-datafield="WorkflowType" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row" style="display: none" id="TemplateDiv">
                <div id="title9" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="Template" style="">打印模版</span>
                </div>
                <div id="control9" class="col-md-4">
                    <%-- <select data-datafield="Template" data-type="SheetDropDownList" id="Select1" class=""
                         style="display:none"  data-schemacode="TemplateFile" data-querycode="GetTemplateName" data-filter="WorkflowType:InstanceType"
                         data-datavaluefield="ObjectID" data-datatextfield="TemplateName" 
                        onchange="TemplateChange()">
                    </select>--%>
                    <select name="select2" id="select2" style="width: 200px" onchange="TemplateChange()">
                        <option value="" selected="selected">《——请选择——》</option>
                    </select>
                </div>
                <div id="space10" class="col-md-2">
                    <input type="button" onclick="jumpToEidtDoc('NTKO', false);" value="打&nbsp印" style="height: 30px; width: 60px" />
                </div>
                <div class="col-md-2" id="NewPrint" style="display: none">
                    <input type="button" onclick="jumpToEidtDoc('NTKO', true);" value="重新打印" style="height: 30px; width: 60px" />
                </div>
                <div id="spaceControl10" class="col-md-2">
                </div>
            </div>
            <div class="row tableContent" style="display: none">
                <div id="title11" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="CheckKeyJson" style="">文件名称Json</span>
                </div>
                <div id="Div11" class="col-md-10">
                    <textarea id="Control16" data-datafield="CheckKeyJson" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
            <div class="row">
                <div id="title363" class="col-md-2">
                    <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
                </div>
                <div id="control363" class="col-md-10">
                    <div id="Control343" data-datafield="SFZ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                    </div>
                </div>
            </div>
            <div>
                <div class="row">
                    <div id="title483" class="col-md-2">
                        <span id="Label428" data-type="SheetLabel" data-datafield="MQ" style="">面签照片</span>
                    </div>
                    <div id="control483" class="col-md-10">
                        <div id="Control428" data-datafield="MQ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title485" class="col-md-2">
                        <span id="Label429" data-type="SheetLabel" data-datafield="mqsp" style="">面签视频</span>
                    </div>
                    <div id="control485" class="col-md-10">
                        <div id="Control429" data-datafield="mqsp" data-type="SheetAttachment" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title377" class="col-md-2">
                        <span id="Label350" data-type="SheetLabel" data-datafield="HT" style="">购车合同</span>
                    </div>
                    <div id="control377" class="col-md-10">
                        <div id="Control350" data-datafield="HT" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title379" class="col-md-2">
                        <span id="Label351" data-type="SheetLabel" data-datafield="GCFP" style="">购车发票</span>
                    </div>
                    <div id="control379" class="col-md-10">
                        <div id="Control351" data-datafield="GCFP" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title381" class="col-md-2">
                        <span id="Label352" data-type="SheetLabel" data-datafield="BD" style="">保单和保单发票</span>
                    </div>
                    <div id="control381" class="col-md-10">
                        <div id="Control352" data-datafield="BD" data-type="SheetAttachment" data-picturecompress="false" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title383" class="col-md-2">
                        <span id="Label353" data-type="SheetLabel" data-datafield="DKHT" style="">贷款合同</span>
                    </div>
                    <div id="control383" class="col-md-10">
                        <div id="Control353" data-datafield="DKHT" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title385" class="col-md-2">
                        <span id="Label354" data-type="SheetLabel" data-datafield="KHDKQEHFSQ" style="">客户贷款全额划付授权书</span>
                    </div>
                    <div id="control385" class="col-md-10">
                        <div id="Control354" data-datafield="KHDKQEHFSQ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title387" class="col-md-2">
                        <span id="Label355" data-type="SheetLabel" data-datafield="KHKKSQS" style="">客户扣款授权书</span>
                    </div>
                    <div id="control387" class="col-md-10">
                        <div id="Control355" data-datafield="KHKKSQS" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title389" class="col-md-2">
                        <span id="Label356" data-type="SheetLabel" data-datafield="GRDKTYTZS" style="">个人贷款同意通知书</span>
                    </div>
                    <div id="control389" class="col-md-10">
                        <div id="Control356" data-datafield="GRDKTYTZS" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>

                <div class="row">
                    <div id="title393" class="col-md-2">
                        <span id="Label358" data-type="SheetLabel" data-datafield="GDHJY" style="">股东会决议（公牌）</span>
                    </div>
                    <div id="control393" class="col-md-10">
                        <div id="Control358" data-datafield="GDHJY" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title473" class="col-md-2">
                        <span id="Label421" data-type="SheetLabel" data-datafield="cldjz" style="">车辆登记证</span>
                    </div>
                    <div id="control473" class="col-md-10">
                        <div id="Control421" data-datafield="cldjz" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title475" class="col-md-2">
                        <span id="Label422" data-type="SheetLabel" data-datafield="yyzz" style="">营业执照</span>
                    </div>
                    <div id="control475" class="col-md-10">
                        <div id="Control422" data-datafield="yyzz" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title477" class="col-md-2">
                        <span id="Label423" data-type="SheetLabel" data-datafield="fjfqrh" style="">附加费确认函</span>
                    </div>
                    <div id="control477" class="col-md-10">
                        <div id="Control423" data-datafield="fjfqrh" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div class="row">
                    <div id="title361" class="col-md-2">
                        <span id="Label342" data-type="SheetLabel" data-datafield="SQD" style="">申请单资料</span>
                    </div>
                    <div id="control361" class="col-md-10">
                        <div id="Control342" data-datafield="SQD" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title365" class="col-md-2">
                        <span id="Label344" data-type="SheetLabel" data-datafield="JHZ" style="">婚姻类材料</span>
                    </div>
                    <div id="control365" class="col-md-10">
                        <div id="Control344" data-datafield="JHZ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title367" class="col-md-2">
                        <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
                    </div>
                    <div id="control367" class="col-md-10">
                        <div id="Control345" data-datafield="JSZ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title369" class="col-md-2">
                        <span id="Label346" data-type="SheetLabel" data-datafield="JZZ" style="">居住类材料</span>
                    </div>
                    <div id="control369" class="col-md-10">
                        <div id="Control346" data-datafield="JZZ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title371" class="col-md-2">
                        <span id="Label347" data-type="SheetLabel" data-datafield="SR" style="">收入类材料</span>
                    </div>
                    <div id="control371" class="col-md-10">
                        <div id="Control347" data-datafield="SR" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title373" class="col-md-2">
                        <span id="Label348" data-type="SheetLabel" data-datafield="GZ" style="">工作证明\企业类证明</span>
                    </div>
                    <div id="control373" class="col-md-10">
                        <div id="Control348" data-datafield="GZ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title375" class="col-md-2">
                        <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
                    </div>
                    <div id="control375" class="col-md-10">
                        <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title391" class="col-md-2">
                        <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">银行卡</span>
                    </div>
                    <div id="control391" class="col-md-10">
                        <div id="Control357" data-datafield="YHKMFYJ" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                 <div class="row">
                    <div id="title515" class="col-md-2">
                        <span id="Label450" data-type="SheetLabel" data-datafield="sfkpz" style="">首付款凭证</span>
                    </div>
                    <div id="control515" class="col-md-10">
                        <div id="Control450" data-datafield="sfkpz" data-type="SheetAttachment" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title497" class="col-md-2">
                        <span id="Label437" data-type="SheetLabel" data-datafield="fjfiqt" style="">FI其他附件</span>
                    </div>
                    <div id="control497" class="col-md-10">
                        <div id="Control437" data-datafield="fjfiqt" data-type="SheetAttachment" data-picturecompress="true" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title469" class="col-md-2">
                        <span id="Label419" data-type="SheetLabel" data-datafield="yyqtfj" style="">运营其他附件</span>
                    </div>
                    <div id="control469" class="col-md-10">
                        <div id="Control419" data-datafield="yyqtfj" data-type="SheetAttachment" data-picturecompress="true" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <span id="Label35711" data-type="SheetLabel" data-datafield="jfbg" style="">家访报告</span>
                    </div>
                    <div class="col-md-10">
                        <div id="Control35711" data-datafield="jfbg" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
            <label id="Label2" data-en_us="Sheet information">审核信息</label>
        </div>
        <div class="divContent" id="div2">
            <div class="row tableContent">
                <div id="title401" class="col-md-2">
                    <span id="Label362" data-type="SheetLabel" data-datafield="ZSYJj" style="">终审意见</span>
                </div>
                <div id="control401" class="col-md-10">
                    <div data-datafield="zsshzt" data-type="SheetRadioButtonList" id="ctl271485" class="" style="" data-defaultitems="核准;拒绝;有条件核准;取消"></div>
                    <div id="Control362" data-datafield="ZSYJj" data-type="SheetComment" style="">
                    </div>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title399" class="col-md-2">
                    <span id="Label361" data-type="SheetLabel" data-datafield="CSYJ" style="">初审意见</span>
                </div>
                <div id="control399" class="col-md-10">
                    <div data-datafield="yycsshzt" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultitems="核准;拒绝;"></div>

                </div>
            </div>
            <div class="row tableContent">
                <div id="title401" class="col-md-2">
                    <span id="Label362" data-type="SheetLabel" data-datafield="ZSYJj" style="">终审意见</span>
                </div>
                <div id="control401" class="col-md-10">
                    <div data-datafield="yyzsshzt" data-type="SheetRadioButtonList" id="ctl271485" class="" style="" data-defaultitems="核准;拒绝;"></div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        //隐藏附加上传空白行
        function hideFIrow() {
            $("#divattachment .row").each(function () {
                if ($(this).find(">.col-md-2>span").css("display") == "none") {
                    $(this).hide();
                }
            })
        }
        var viewer;
        function chkzhm(ts) {
            $("#Control415").val($(ts).val());
        }
        // 页面加载完成事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            //getApplicantUserInfoByBizObject();
            var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
            var IsWork = $.MvcSheetUI.QueryString("Mode");
            //关于打印加载事件
            SheetLoaded();
            $(".delete").hide();
            if (IsWork == "work") {
                var opt_v = $.MvcSheetUI.SheetInfo.BizObject.DataItems["APPLICATION_NAME"].V;
                $("#Control415s").append("<option value=\"" + opt_v + "\">" + opt_v + "</option>");
                $("#Control415").val(opt_v);
                $("#Control415s").show();
                $("#Control415s").parent().find(">input").hide();
                if ($("#Control415").val() && $("#Control415").val().length > 0) {
                    $("#Control415s").val($("#Control415").val());
                }
            }
            else {
                $("#btn_update_to_cap").remove();
            }
            hideFIrow();
            getmsg();
            //风控审核结果转换
            var arrrsrq = [];
            arrrsrq['localreject'] = "东正本地规则<span style=\"color:red;\">拒绝</span>";
            arrrsrq['cloudaccept'] = "云端规则<span style=\"color:red;\">通过</span>";
            arrrsrq['cloudreject'] = "云端规则<span style=\"color:red;\">拒绝</span>";
            arrrsrq['cloudmanual'] = "云端规则返回<span style=\"color:red;\">转人工</span>";
            arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
            $("#Div3").find("span").html(arrrsrq[$("#Div3").find("span").html()]);
            //添加留言
            $('#addmsga').on('click', function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
        }
        //确认提交后事件
        $.MvcSheet.AfterConfirmSubmit = function () {
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity48") {
                return update_to_cap(false);
            }
            return true;
        }
        // 表单验证接口
        $.MvcSheet.Validate = function () {
            if (this.Action == "Submit" || this.Action == "Reject") {
                var isdayend = checkIsDayend();
                if (!isdayend.Success) {
                    shakeMsg(isdayend.Message);
                    return false;
                }
            }
            var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
            if (this.Action == "Submit") {
                return validate_data_to_cap();
            }
        }

        function update_to_cap(need_validate) {
            if (need_validate) {
                if (!validate_data_to_cap()) {
                    return false;
                }
            }
            var applicationNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
            var AccountName = $.MvcSheetUI.GetControlValue("Accoutname");
            var AccountNumber = $.MvcSheetUI.GetControlValue("AccoutNumber");
            var BankName = $.MvcSheetUI.GetControlValue("Bankname");
            var BranchName = $.MvcSheetUI.GetControlValue("Branchname");
            //var EngineNo = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ENGINE", 1);
            //var VinNo = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.VIN_NUMBER", 1);
            var EngineNo = $.MvcSheetUI.GetControlValue("engine_number");
            var VinNo = $.MvcSheetUI.GetControlValue("vin_number");
            var v = $.MvcSheetUI.MvcRuntime.executeService('CAPService', 'UpdateDDBank',
                {
                    applicationNumber: applicationNumber,
                    AccountName: AccountName,
                    AccountNumber: AccountNumber,
                    BankName: BankName,
                    BranchName: BranchName,
                    EngineNo: EngineNo,
                    VinNo: VinNo
                }
            );
            if (!v || v.toUpperCase().indexOf("SUCCESS") == "-1") {
                shakeMsg("CAP错误:" + v);
                return false;
            }
            if (need_validate) {
                $("li[data-action=\"Save\"]").click();
                showMsg("更新成功:" + v);
            }
            console.log(v);
            return true;
        }

        //更新车架号、银行信息到CAP系统；
        function validate_data_to_cap() {
            var applicationNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
            var AccountName = $.MvcSheetUI.GetControlValue("Accoutname");
            var AccountNumber = $.MvcSheetUI.GetControlValue("AccoutNumber");
            var BankName = $.MvcSheetUI.GetControlValue("Bankname");
            var BranchName = $.MvcSheetUI.GetControlValue("Branchname");
            //var EngineNo = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ENGINE", 1);
            //var VinNo = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.VIN_NUMBER", 1);
            var EngineNo = $.MvcSheetUI.GetControlValue("engine_number");
            var VinNo = $.MvcSheetUI.GetControlValue("vin_number");
            if (applicationNumber == "" || applicationNumber == "-1") {
                shakeMsg("申请号错误，请联系管理员");
                return false;
            }
            else if (AccountName == "") {
                shakeMsg("请输入/选择账户名");
                return false;
            }
            else if (AccountNumber == "") {
                shakeMsg("请输入账户号");
                return false;
            }
            else if (AccountNumber.length < 16) {
                shakeMsg("账户号至少16位");
                return false;
            }
            else if (BankName == "") {
                shakeMsg("请选择扣款银行");
                return false;
            }
            else if (BranchName == "") {
                shakeMsg("请选择银行分支");
                return false;
            }
            else if (VinNo == "") {
                shakeMsg("请输入VIN码");
                return false;
            }
            return true;
        }

        $.MvcSheet.PreInit = function () {
            var rejectActivities = $.MvcSheetUI.SheetInfo.RejectActivities;
            var submitActivities = $.MvcSheetUI.SheetInfo.SubmitActivities;

            //驳回选择项大于1，进行过滤
            if (rejectActivities.length > 1) {
                rejectActivities.forEach(function (v, index) {
                    //如果有二个驳回，说明是没有走自动审批，去掉手工的驳回选项；
                    if (v.Code == "Activity8")
                        rejectActivities.splice(index, 1);
                });
            }
            if (submitActivities.length > 1) {
                var lastItem = null;
                $.ajax({
                    type: "post",
                    //url: "/Portal/ajax/WorkItemHandler.ashx?CommandName=getLastWorkItem&id=" + $.MvcSheetUI.SheetInfo.WorkItemId,
                    url: "/Portal/WorkItemHandler/getLastWorkItem?CommandName=getLastWorkItem&id=" + $.MvcSheetUI.SheetInfo.WorkItemId,// 19.6.28 wangxg
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    dataType: "json",
                    success: function (data) {
                        if (data.Success)
                            lastItem = data.Data;
                        else
                            shakeMsg(data.Msg);
                    },
                    error: function (msg) {// 19.7 
					    showJqErr(msg);
                    }
                });
                if (lastItem) {
                    //上一条任务为驳回的，只能提交到驳回的那个节点，不能驳回到其它的节点
                    if (lastItem.ActionEventType == 3) {
                        submitActivities.forEach(function (v, index) {
                            //把编码不为驳回节点的提交按钮去除；
                            if (v.Code != lastItem.ActivityCode)
                                submitActivities.splice(index, 1);
                        });
                    }
                }
            }
        }
    </script>
</asp:Content>
