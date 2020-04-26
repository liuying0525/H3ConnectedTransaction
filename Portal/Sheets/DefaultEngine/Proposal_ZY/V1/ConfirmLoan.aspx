<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfirmLoan.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.ConfirmLoan" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link href="css/common.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" rel="stylesheet" />
    <script type="text/javascript" src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>006"></script>
    <script type="text/javascript" src="js/Validate.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>001"></script>
    <script type="text/javascript" src="/Portal/Custom/js/ocr.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>005"></script>
    <script type="text/javascript" src="/Portal/WFRes/layer/layer.js"></script>
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

        .chkbank select {
            display: block !important;
        }

        .chkbank span {
            display: none !important;
        }

        .tab-content {
            padding: 0px;
        }

        #pmt_detail .modal-dialog {
            width: 800px;
        }

        #pmt_detail table {
            margin-bottom: 0px;
        }

        #pmt_detail table td, th {
            text-align: center;
            width: 16.6%;
        }
    </style>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">个人汽车贷款申请</label>
    </div>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divBaseInfo',this)">
            <label data-en_us="Basic information">基本信息</label>
        </div>
        <div class="divContent" id="divSheet" style="padding-right: 0%">
            <div class="row">
                <div class="col-md-4">
                    <div id="title390" class="leftbox">
                        <span id="Label470" data-type="SheetLabel" data-datafield="APPLICATION_NUMBER" style="">贷款申请编号</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="Div600" class="rightbox">
                        <input id="Control470" type="text" data-datafield="APPLICATION_NUMBER" disabled data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="divOriginateDateTitle" class="leftbox">
                        <label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">日期</label>
                    </div>
                    <div class="centerline"></div>
                    <div id="divOriginateDate" class="rightbox">
                        <label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style="">
                        </label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <div id="title1" class="leftbox">
                        <span id="Label11" data-type="SheetLabel" data-datafield="BP_COMPANY_ID" style="">申请贷款公司</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="control1" class="rightbox">
                        <select id="Text1" data-datafield="BP_COMPANY_ID" data-type="SheetDropDownList" data-masterdatacategory="申请贷款公司" style="" data-queryable="false"></select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title50" class="leftbox2">
                        <span id="Label58" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_CODE" style="">申请类型</span>
                    </div>
                    <div class="centerline2"></div>
                    <div id="Div75" class="rightbox2">
                        <select id="Control58" data-datafield="APPLICATION_TYPE_CODE" data-type="SheetDropDownList" style="" data-masterdatacategory="申请类型" data-queryable="false"></select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title23" class="leftbox3">
                        <span id="Label32" data-type="SheetLabel" data-datafield="BP_SALESPERSON_ID" style="">销售人员</span>
                    </div>
                    <div class="centerline3"></div>
                    <div id="Div45" class="rightbox3">
                        <select id="Control32" data-datafield="BP_SALESPERSON_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_sale_man" data-querycode="002" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <div id="title39" class="leftbox">
                        <span id="Label47" data-type="SheetLabel" data-datafield="BUSINESS_PARTNER_ID" style="">经销商代码</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="Div60" class="rightbox">
                        <select id="Control47" data-datafield="BUSINESS_PARTNER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_business_partner_id" data-querycode="003" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title63" class="leftbox2">
                        <span id="Label69" data-type="SheetLabel" data-datafield="STATE_CDE" style="">省</span>
                    </div>
                    <div class="centerline2"></div>
                    <div id="Div86" class="rightbox2">
                        <select id="Control69" data-datafield="STATE_CDE" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_Province" data-querycode="004" data-datavaluefield="STATE_CDE" data-datatextfield="STATE_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title64" class="leftbox3">
                        <span id="Label70" data-type="SheetLabel" data-datafield="CITY_CDE" style="">城市</span>
                    </div>
                    <div class="centerline3"></div>
                    <div id="Div87" class="rightbox3">
                        <select id="Control70" data-datafield="CITY_CDE" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_City" data-querycode="005" data-datavaluefield="CITY_CDE" data-datatextfield="CITY_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <div id="Div12" class="leftbox">
                        <span id="Span3" data-type="SheetLabel" data-datafield="VEHICLE_TYPE_CDE" style="">车辆类型</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="Div13" class="rightbox">
                        <select id="Text4" data-datafield="VEHICLE_TYPE_CDE" data-type="SheetDropDownList" style=""
                            data-schemacode="M_CarType" data-querycode="001" data-datavaluefield="VEHICLE_TYPE_CDE" data-datatextfield="VEHICLE_TYPE_DSC" data-selectedvalue="00001">
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="Div20" class="leftbox2">
                        <span id="Span7" data-type="SheetLabel" data-datafield="BP_DEALER_ID" style="">金融顾问</span>
                    </div>
                    <div class="centerline2"></div>
                    <div id="Div21" class="rightbox2">
                        <select id="Text8" data-datafield="BP_DEALER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_Financial_Adviser" data-querycode="006" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title26" class="leftbox3">
                        <span id="Label35" data-type="SheetLabel" data-datafield="CONTACT_PERSON" style="">联系方式</span>
                    </div>
                    <div class="centerline3"></div>
                    <div id="Div48" class="rightbox3">
                        <input id="Control35" type="text" data-datafield="CONTACT_PERSON" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'30')">
                    </div>
                </div>
            </div>
            <div class="row">
                <%--制造先注释，没有找到对应的字段--%>
                <%--<div class="col-md-4">
                    <div id="title11" class="leftbox">
                        <span id="Label21" data-type="SheetLabel" data-datafield="distributorName" style="">制造商</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="control11" class="rightbox">
                        <select id="Control21" data-datafield="distributorName" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_manufacturers" data-querycode="008" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id"></select>
                    </div>
                </div>--%>
                <div class="col-md-4">
                    <div id="title58" class="leftbox">
                        <span id="Label65" data-type="SheetLabel" data-datafield="BP_SHOWROOM_ID" style="">展厅</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="Div82" class="rightbox">
                        <select id="Control65" data-datafield="BP_SHOWROOM_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_exhibition_hall" data-querycode="007" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title49" class="leftbox2">
                        <span id="Label57" data-type="SheetLabel" data-datafield="REFERENCE_NBR" style="">申请参考号</span>
                    </div>
                    <div class="centerline2"></div>
                    <div id="Div74" class="rightbox2">
                        <input id="Control57" type="text" data-datafield="REFERENCE_NBR" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'20')">
                    </div>
                </div>
            </div>
            <div class="row" cus_type="sqr">
                <div id="title21" class="col-md-2">
                    <label>
                        <span id="Label29" data-type="SheetLabel" style="">姓名（中文）</span>
                    </label>
                </div>
                <div id="sqr_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div id="title23" class="col-md-2">
                    <label>
                        <span id="Label31" data-type="SheetLabel" style="">证件号码</span>
                    </label>
                </div>
                <div id="sqr_id" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="sqr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">公司名称</span>
                    </label>
                </div>
                <div id="sqr_company_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">组织机构代码</span>
                    </label>
                </div>
                <div id="sqr_company_org_code" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="sqr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">注册号</span>
                    </label>
                </div>
                <div id="sqr_company_sts" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="gjr">
                <div id="title123" class="col-md-2">
                    <label>
                        <span id="Label122" data-type="SheetLabel" style="">共借人姓名（中文）</span>
                    </label>
                </div>
                <div id="gjr_name" class="col-md-4">
                    <label><span></span></label>
                </div>
                <div id="title125" class="col-md-2">
                    <label>
                        <span id="Label124" style="">共借人证件号码</span>
                    </label>
                </div>
                <div id="gjr_id" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="gjr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">共借公司名称</span>
                    </label>
                </div>
                <div id="gjr_company_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">共借公司组织机构代码</span>
                    </label>
                </div>
                <div id="gjr_company_org_code" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="gjr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">共借公司注册号</span>
                    </label>
                </div>
                <div id="gjr_company_sts" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="dbr">
                <div id="title217" class="col-md-2">
                    <label>
                        <span id="Label209" data-type="SheetLabel" style="">担保人姓名</span>
                    </label>
                </div>
                <div id="dbr_name" class="col-md-4">
                    <label><span></span></label>
                </div>
                <div id="title219" class="col-md-2">
                    <label>
                        <span id="Label211" data-type="SheetLabel" style="">担保人证件号码</span>
                    </label>
                </div>
                <div id="dbr_id" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="dbr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">担保公司名称</span>
                    </label>
                </div>
                <div id="dbr_company_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">担保公司组织机构代码</span>
                    </label>
                </div>
                <div id="dbr_company_org_code" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="dbr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">担保公司注册号</span>
                    </label>
                </div>
                <div id="dbr_company_sts" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row hidden">
                <div id="title421" class="col-md-2">
                    <span id="Label379" data-type="SheetLabel"  style="">风控审核结果</span>
                </div>
                <div id="Div3" class="col-md-4">
                    
                </div>
            </div>
            <div class="row hidden">
                <div class="col-md-4">
                    <div id="title48101" class="leftbox">
                        <span>APPLICATION_TYPE_NAME</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="Div48101" class="rightbox">
                        <input id="Control48101" type="text" data-datafield="APPLICATION_TYPE_NAME" data-type="SheetTime" style="" class="">
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title48131" class="leftbox">
                        <span>APPLICATION_NAME</span>
                    </div>
                    <div class="centerline"></div>
                    <div id="Div48131" class="rightbox">
                        <input id="Control48131" type="text" data-datafield="APPLICATION_NAME" data-type="SheetTime" style="" class="">
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title491" class="leftbox3">
                        <span id="Label571" data-type="SheetLabel" data-datafield="APPLICATION_DATE" style="">APPLICATION_DATE</span>
                    </div>
                    <div class="centerline3"></div>
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
                    <div class="centerline"></div>
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
                    <div class="centerline"></div>
                    <div class="rightbox">
                        <input type="text" id="hidden_001" data-datafield="bp_id" data-type="SheetTextBox" style="" />
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="leftbox">
                        USER_NAME
                    </div>
                    <div class="centerline"></div>
                    <div class="rightbox">
                        <input type="text" id="hidden_002" data-datafield="USER_NAME" data-type="SheetTextBox" style="" data-defaultvalue="{Originator.LoginName}" />
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="leftbox">
                        PLAN_ID
                    </div>
                    <div class="centerline"></div>
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
                    <div class="centerline"></div>
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
                    <div class="centerline"></div>
                    <div class="rightbox">
                        <textarea id="Textarea1" data-datafield="cyzd" data-type="SheetRichTextBox" class="" style="">					</textarea>
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title445" class="col-md-12">
                    <span id="Label398" data-type="SheetLabel" data-datafield="LY" style="">留言信息</span>
                </div>
            </div>
            <div class="row tableContent">
                <div id="control445" class="col-md-12">
                    <table class="SheetGridView" style="text-align: center; width: 100%;">
                        <tbody id="liuyan">
                            <tr class="header">
                                <td class="rowSerialNo">序号</td>
                                <td>
                                    <label>留言内容</label>
                                </td>
                                <td style="width: 120px;">
                                    <label>留言人员</label>
                                </td>
                                <td style="width: 135px;">
                                    <label>留言时间</label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-right bannerTitle" id="titleAsset" onclick="hidediv('div_Detail_VEHICLE',this)">
            <span>资产信息</span>
        </div>
        <div class="divContent" id="div_Detail_VEHICLE" style="padding-right: 0%">
            <div class="row tableContent">
                <div id="div665779" class="col-md-12">
                    <div id="ctl524044" data-datafield="VEHICLE_DETAIL_ZY" data-type="SheetDetail" class="" style=""
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
                                            <select data-datafield="VEHICLE_DETAIL_ZY.CONDITION" data-type="SheetDropDownList" id="ctl524044_control9" style=""
                                                data-masterdatacategory="资产状况" data-queryable="false" data-onchange="asset_Condition_Change(this)">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title7" class="col-md-4">
                                            <span id="ctl524044_label7" style="">购车目的</span>
                                        </div>
                                        <div id="ctl524044_Data7" class="col-md-8">
                                            <select data-datafield="VEHICLE_DETAIL_ZY.USAGE7" data-type="SheetDropDownList" id="ctl524044_control7" style=""
                                                data-masterdatacategory="购车目的" data-queryable="false">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title3" class="col-md-4">
                                            <span id="ctl524044_label3" style="">车辆品牌</span>
                                        </div>
                                        <div id="ctl524044_Data3" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ASSET_MAKE_DSC" disabled data-type="SheetTextBox" id="ctl524044_control3" style="width: 80%" data-popupheight="500px" data-popupwidth="800px"
                                                data-schemacode="M_VEHICLE" data-querycode="fun_M_VEHICLE" data-popupwindow="PopupWindow" data-inputmappings="VEHICLE_DETAIL_ZY.ASSET_MAKE_CDE:asset_make_cde,VEHICLE_DETAIL_ZY.ASSET_BRAND_CDE:asset_brand_cde,VEHICLE_DETAIL_ZY.ASSET_MODEL_CDE:asset_model_cde,bp_id:bp_id,VEHICLE_DETAIL_ZY.CONDITION:asset_condition"
                                                data-outputmappings="VEHICLE_DETAIL_ZY.ASSET_MAKE_CDE:asset_make_cde,VEHICLE_DETAIL_ZY.ASSET_MAKE_DSC:asset_make_dsc,VEHICLE_DETAIL_ZY.ASSET_BRAND_CDE:asset_brand_cde,VEHICLE_DETAIL_ZY.ASSET_BRAND_DSC:asset_brand_dsc,VEHICLE_DETAIL_ZY.ASSET_MODEL_CDE:asset_model_cde,VEHICLE_DETAIL_ZY.COMMENTS7:asset_model_dsc,VEHICLE_DETAIL_ZY.POWER_PARAMETER:asset_model_dsc,VEHICLE_DETAIL_ZY.MIOCN_ID:miocn_id,VEHICLE_DETAIL_ZY.MIOCN_NBR:miocn_nbr,VEHICLE_DETAIL_ZY.MIOCN_DSC:miocn_dsc,VEHICLE_DETAIL_ZY.NEW_PRICE:retail_price_amt,CONTRACT_DETAIL_ZY.TOTAL_ASSET_COST:retail_price_amt,VEHICLE_DETAIL_ZY.VEHICLE_TYPE_CDE:vehicle_type_cde,VEHICLE_DETAIL_ZY.VEHICLE_SUBTYPE_CDE:vehicle_subtyp_cde,VEHICLE_DETAIL_ZY.TRANSMISSION:transmission_type_cde">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title75" class="col-md-4">
                                            <span id="ctl524044_label75" style="">车系</span>
                                        </div>
                                        <div id="ctl524044_Data75" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ASSET_BRAND_DSC" disabled data-type="SheetTextBox" id="ctl524044_control75" style="">
                                        </div>
                                    </div>

                                    <div class="col-md-4">
                                        <div id="ctl524044_Title73" class="col-md-4">
                                            <span id="ctl524044_label73" style="">车型</span>
                                        </div>
                                        <div id="ctl524044_Data73" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.POWER_PARAMETER" disabled style="" data-type="SheetTextBox" id="ctl524044_control73" 
                                                data-onchange="power_parameter_chg(this)"/>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title79" class="col-md-4">
                                            <span id="ctl524044_label79" style="">资产编码</span>
                                        </div>
                                        <div id="ctl524044_Data79" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.MIOCN_NBR" readonly data-type="SheetTextBox" id="ctl524044_control79" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title22" class="col-md-4">
                                            <span id="ctl524044_label22" style="">变速器</span>
                                        </div>
                                        <div id="ctl524044_Data22" class="col-md-8">
                                            <select data-datafield="VEHICLE_DETAIL_ZY.TRANSMISSION" data-type="SheetDropDownList" id="ctl524044_control22" style=""
                                                data-masterdatacategory="变速器" data-displayemptyitem="true" data-queryable="false">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title51" class="col-md-4">
                                            <span id="ctl524044_label51" style="">车身颜色</span>
                                        </div>
                                        <div id="ctl524044_Data51" class="col-md-8">
                                            <select data-datafield="VEHICLE_DETAIL_ZY.COLOR" disabled data-type="SheetDropDownList" id="ctl524044_control51" style="" data-queryable="false"
                                                data-schemacode="M_asset_color" data-querycode="fun_M_asset_color" data-datavaluefield="COLOR_CDE" data-datatextfield="COLOR_DSC" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title24" class="col-md-4">
                                            <span id="ctl524044_label24" style="">资产价格</span>
                                        </div>
                                        <div id="ctl524044_Data24" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.NEW_PRICE" readonly data-type="SheetTextBox" id="ctl524044_control24" style="" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title82" class="col-md-4">
                                            <span id="ctl524044_label82" style="">制造年份</span>
                                        </div>
                                        <div id="ctl524044_Data82" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.MANUFACTURE_YEAR" data-type="SheetTextBox" id="ctl524044_control82" style="" onkeyup="this.value=strMaxLength(this.value,'4')" maxlength="4">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title14" class="col-md-4">
                                            <span id="ctl524044_label14" style="">TBR日期</span>
                                        </div>
                                        <div id="ctl524044_Data14" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.BUILD_DATE" data-type="SheetTime" id="ctl524044_control14" style=""
                                                data-defaultvalue="">
                                        </div>
                                    </div>
                                   
                                </div>
                                <div class="row">
                                     <div class="col-md-4">
                                        <div id="ctl524044_Title77" class="col-md-4">
                                            <span id="ctl524044_label77" style="">出厂日期</span>
                                        </div>
                                        <div id="ctl524044_Data77" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.RELEASE_DTE" data-type="SheetTime" id="ctl524044_control77" style=""
                                                data-defaultvalue="" data-onchange="releaseDateChange(this)">
                                        </div>
                                    </div>
                                    <div class="col-md-8">
                                        <div id="ctl524044_Title83" class="col-md-4">
                                            <span id="ctl524044_label83" style="">担保方式</span>
                                        </div>
                                        <div id="ctl524044_Data83" class="col-md-8">
                                            <%-- <textarea data-datafield="VEHICLE_DETAIL_ZY.GURANTEE_OPTION" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl524044_control83">											</textarea>--%>
                                            <div data-datafield="VEHICLE_DETAIL_ZY.GURANTEE_OPTION_H3" data-type="SheetCheckboxList" id="ctl844175" class="" style="" data-repeatdirection="Vertical" data-repeatcolumns="2" data-masterdatacategory="担保方式" data-onchange="gurantee_option_chg()"></div>
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title85" class="col-md-4">
                                            <span id="ctl524044_label85" style="">借款用途</span>
                                        </div>
                                        <div id="ctl524044_Data85" class="col-md-8">
                                            <div>自用购车</div>
                                            <%--<select data-datafield="VEHICLE_DETAIL_ZY.LoanUsage" data-type="SheetDropDownList" id="ctl8441755" style="" data-vaildationrule="{MARITAL_STATUS_CDE}=='0'" >
                                                <option value="0" selected>自用购车</option>
                                            </select>--%>
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title19" class="col-md-4">
                                            <span id="ctl524044_label19" style="">发动机号码</span>
                                        </div>
                                        <div id="ctl524044_Data19" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ENGINE" data-type="SheetTextBox" id="ctl524044_control19" style="" onkeyup="this.value=strMaxLength(this.value,'40')" maxlength="40">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title28" class="col-md-4">
                                            <span id="ctl524044_label28" style="">VIN码</span>
                                        </div>
                                        <div id="ctl524044_Data28" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.VIN_NUMBER" readonly data-type="SheetTextBox" maxlength="17" id="ctl524044_control28"
                                                style="" data-onchange="vin_change(this)" onkeyup="this.value=strMaxLength(this.value,'25')">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div id="ctl524044_Title45" class="col-md-4" style="width: 11.11%">
                                            <span id="ctl524044_label45" style="">备注</span>
                                        </div>
                                        <div id="ctl524044_Data45" class="col-md-8" style="width: 88%">
                                            <textarea data-datafield="VEHICLE_DETAIL_ZY.COMMENTS7" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl524044_control45" 
                                                onkeyup="this.value=strMaxLength(this.value,'255')" maxlength="200"></textarea>
                                        </div>
                                    </div>
                                </div>
                                <%--<div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title31" class="col-md-4">
                                            <span id="ctl524044_label31" style="">注册号</span>
                                        </div>
                                        <div id="ctl524044_Data31" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.REGISTRATION_NUMBER" data-type="SheetTextBox" id="ctl524044_control31" style="" onkeyup="this.value=strMaxLength(this.value,'25')" maxlength="25">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title32" class="col-md-4">
                                            <span id="ctl524044_label32" style="">车辆使用年数</span>
                                        </div>
                                        <div id="ctl524044_Data32" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.VEHICLE_AGE" data-defaultvalue="0" data-type="SheetTextBox" id="ctl524044_control32" style="" onkeyup="value=value.replace(/[^\d\.]/g,'')" maxlength="3">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title13" class="col-md-4">
                                            <span id="ctl524044_label13" style="">出厂年份 </span>
                                        </div>
                                        <div id="ctl524044_Data13" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.RELEASE_YEAR" data-type="SheetTextBox" readonly id="ctl524044_control13" style="" onkeyup="this.value=strMaxLength(this.value,'4')" maxlength="4">
                                        </div>
                                    </div>
                                </div>--%>
                                <div class="row bottom">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title15" class="col-md-4">
                                            <span id="ctl524044_label15" style="">里程表</span>
                                        </div>
                                        <div id="ctl524044_Data15" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ODOMETER_READING" data-type="SheetTextBox" id="ctl524044_control15" style="" onkeyup="this.value=strMaxLength(this.value,'5')" maxlength="5">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title13" class="col-md-4">
                                            <span id="ctl524044_label13" style="">出厂年份 </span>
                                        </div>
                                        <div id="ctl524044_Data13" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.RELEASE_YEAR" data-type="SheetTextBox" readonly id="ctl524044_control13" style="" onkeyup="this.value=strMaxLength(this.value,'4')" maxlength="4">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title16" class="col-md-4">
                                            <span id="ctl524044_label16" style="">出厂月份</span>
                                        </div>
                                        <div id="ctl524044_Data16" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.RELEASE_MONTH" readonly data-type="SheetTextBox" id="ctl524044_control16" style="" onkeyup="this.value=strMaxLength(this.value,'3')" maxlength="3">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl524044_Title18" class="col-md-4">
                                            <span id="ctl524044_label18" style="">系列</span>
                                        </div>
                                        <div id="ctl524044_Data18" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.SERIES" data-type="SheetTextBox" id="ctl524044_control18" style="" onkeyup="this.value=strMaxLength(this.value,'40')" maxlength="40">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title23" class="col-md-4">
                                            <span id="ctl524044_label23" style="">汽缸</span>
                                        </div>
                                        <div id="ctl524044_Data23" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.CYLINDER" data-type="SheetTextBox" id="ctl524044_control23" style="" onkeyup="this.value=strMaxLength(this.value,'10')" maxlength="10">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title27" class="col-md-4">
                                            <span id="ctl524044_label27" style="">轮宽</span>
                                        </div>
                                        <div id="ctl524044_Data27" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.WHEEL_WIDTH" data-type="SheetTextBox" id="ctl524044_control27" style="" onkeyup="this.value=strMaxLength(this.value,'10')" maxlength="10">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title61" class="col-md-4">
                                            <span id="ctl524044_label61" style="">风格</span>
                                        </div>
                                        <div id="ctl524044_Data61" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.STYLE" data-type="SheetTextBox" id="ctl524044_control61" style="" onkeyup="this.value=strMaxLength(this.value,'3')" maxlength="3">
                                        </div>
                                    </div>
                                </div>


                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title2" class="col-md-4">
                                            <span id="ctl524044_label2" style="">IDENTIFICATION_CODE</span>
                                        </div>
                                        <div id="ctl524044_Data2" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.IDENTIFICATION_CODE7" data-type="SheetTextBox" id="ctl524044_control2" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524045_Title3" class="col-md-4">
                                            <span id="ctl524045_label3" style="">制造商Code</span>
                                        </div>
                                        <div id="ctl524045_Data3" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ASSET_MAKE_CDE" data-type="SheetTextBox" id="ctl524045_control3" style="" />
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524045_Title75" class="col-md-4">
                                            <span id="ctl524045_label75" style="">车型Code</span>
                                        </div>
                                        <div id="ctl524045_Data75" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ASSET_BRAND_CDE" data-type="SheetTextBox" id="ctl524045_control75" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title71" class="col-md-4">
                                            <span id="ctl524044_label71" style="">VEHICLE_TYPE_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data71" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.VEHICLE_TYPE_CDE" data-type="SheetTextBox" id="ctl524044_control71" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title72" class="col-md-4">
                                            <span id="ctl524044_label72" style="">VEHICLE_SUBTYPE_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data72" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.VEHICLE_SUBTYPE_CDE" data-type="SheetTextBox" id="ctl524044_control72" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title4" class="col-md-4">
                                            <span id="ctl524044_label4" data-type="SheetLabel" style="">ASSET_MODEL_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data4" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.ASSET_MODEL_CDE" data-type="SheetTextBox" id="ctl524044_control4" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title78" class="col-md-4">
                                            <span id="ctl524044_label78" style="">资产ID</span>
                                        </div>
                                        <div id="ctl524044_Data78" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.MIOCN_ID" data-type="SheetTextBox" id="ctl524044_control78" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title80" class="col-md-4">
                                            <span id="ctl524044_label80" style="">MIOCN_DSC</span>
                                        </div>
                                        <div id="ctl524044_Data80" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.MIOCN_DSC" data-type="SheetTextBox" id="ctl524044_control80" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl5240414_Title83" class="col-md-4">
                                            <span id="ctl5240414_label83" style="">担保方式</span>
                                        </div>
                                        <div id="ctl5240414_Data83" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.GURANTEE_OPTION" data-type="SheetTextBox" id="ctl8441175" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-12">
                                        <div id="ctl5245044_Title45" class="col-md-4" style="width: 11.11%">
                                            <span id="ctl5245044_label45" style="">备注</span>
                                        </div>
                                        <div id="ctl5245044_Data45" class="col-md-8" style="width: 88%">
                                            <textarea data-datafield="VEHICLE_DETAIL_ZY.VEHICLE_COMMENT" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl5245044_control45" onkeyup="this.value=strMaxLength(this.value,'255')" maxlength="200"></textarea>
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title70" class="col-md-4">
                                            <span id="ctl524044_label70" style="">车身</span>
                                        </div>
                                        <div id="ctl524044_Data70" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL_ZY.VEHICLE_BODY" data-type="SheetTextBox" id="ctl524044_control70" style="" onkeyup="this.value=strMaxLength(this.value,'30')" maxlength="30">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <div class="col-md-4">
                        <span data-type="SheetLabel" data-datafield="Bankname" style="">发动机号码</span>
                    </div>
                    <div class="col-md-8">
                        <input type="text" data-datafield="engine_number" data-type="SheetTextBox" id="ctl_engine_number" class="" style="" 
                            onkeyup="this.value=strMaxLength(this.value,'40')" maxlength="40"/>
                    </div>
                </div>
                <div class="col-md-8">
                    <div class="col-md-4" style="width:16.67%">
                        <span data-type="SheetLabel" data-datafield="Branchname" style="">VIN码</span>
                    </div>
                    <div class="col-md-8">
                        <input type="text" data-datafield="vin_number" data-type="SheetTextBox" id="ctl_vin_number" class="" style="" 
                            data-onchange="vin_change_fi(this)" maxlength="17"/>
                    </div>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title185" class="col-md-2">
                    <span id="Label145" data-type="SheetLabel" data-datafield="ASSET_ACCESSORY_ZY" style="">附加费明细表</span>
                </div>
                <div id="control185" class="col-md-10">
                    <table id="Control145" data-datafield="ASSET_ACCESSORY_ZY" data-type="SheetGridView" class="SheetGridView">
                        <tbody>
                            <tr class="header">
                                <td id="Control145_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control145_Header3" data-datafield="ASSET_ACCESSORY_ZY.ACCESSORY_CDE">
                                    <label id="Control145_Label3" data-datafield="ASSET_ACCESSORY_ZY.ACCESSORY_CDE" data-type="SheetLabel" style="">选装部件</label>
                                </td>
                                <td id="Control145_Header4" data-datafield="ASSET_ACCESSORY_ZY.PRICE" style="width: 180px">
                                    <label id="Control145_Label4" data-datafield="ASSET_ACCESSORY_ZY.PRICE" data-type="SheetLabel" style="">价格</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control145_Option" class="rowOption"></td>
                                <td data-datafield="ASSET_ACCESSORY_ZY.ACCESSORY_CDE">
                                    <select id="Control145_ctl3" data-datafield="ASSET_ACCESSORY_ZY.ACCESSORY_CDE" data-type="SheetDropDownList" style="width: 92%" data-displayemptyitem="true"
                                        data-schemacode="M_accessory_code" data-querycode="fun_M_accessory_code" data-datavaluefield="ACCESSORY_CDE" data-datatextfield="ACCESSORY_DSC"
                                        data-onchange="accessory_chg(this)">
                                    </select>
                                </td>
                                <td data-datafield="ASSET_ACCESSORY_ZY.PRICE">
                                    <input id="Control145_ctl4" type="text" data-datafield="ASSET_ACCESSORY_ZY.PRICE" data-type="SheetTextBox" style="width: 92%" data-formatrule="{0:N2}"
                                        data-regularexpression="/^[+]{0,1}(\d+)$|^[+]{0,1}(\d+\.\d+)$/" data-regularinvalidtext="请输入正数" maxlength="9">
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
            <div class="row">
                <div id="title461" class="col-md-2">
                    <span id="Label413" data-type="SheetLabel" data-datafield="Cbankname" style="">客户银行</span>
                </div>
                <div id="control461" class="col-md-4">
                    <input id="Control413" type="text" data-datafield="Cbankname" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'50')" maxlength="25">
                </div>
                <%--<div id="title462" class="col-md-2">
					<span id="Label414" data-type="SheetLabel" data-datafield="Cbranchname" style="">银行分支</span>
				</div>
				<div id="control462" class="col-md-4">
					<input id="Control414" type="text" data-datafield="Cbranchname" data-type="SheetTextBox" style="">
				</div>--%>
                <div id="Div1" class="col-md-2">
                    <span id="Span1" data-type="SheetLabel" data-datafield="Caccountnum" style="">账户号</span>
                </div>
                <div id="Div3" class="col-md-4">
                    <input id="inp_Caccountnum" oninput="textnum(this)" type="text" data-datafield="Caccountnum" data-type="SheetTextBox" style="" maxlength="20">
                </div>
            </div>
            <%--<div class="row">
				<div id="title463" class="col-md-2">
					<span id="Label415" data-type="SheetLabel" data-datafield="Caccountname" style="">账户名</span>
				</div>
				<div id="control463" class="col-md-4">
					<input id="Control415" type="text" data-datafield="Caccountname" data-type="SheetTextBox" style="">
				</div>
				<div id="title464" class="col-md-2">
					<span id="Label416" data-type="SheetLabel" data-datafield="Caccountnum" style="">账户号</span>
				</div>
				<div id="control464" class="col-md-4">
					<input id="Control416" type="text" data-datafield="Caccountnum" data-type="SheetTextBox" style="">
				</div>
			</div>--%>
        </div>


        <%-- 金融条款表 --%>
        <div class="nav-icon fa  fa-chevron-right bannerTitle" id="titleJRTK" onclick="hidediv('div_Detail_CONTRACT',this)">
            <span>金融条款</span>
        </div>
        <div class="divContent" id="div_Detail_CONTRACT" style="padding-right: 0%">
            <div class="row tableContent">
                <div id="div505260" class="col-md-12">
                    <div id="ctl508406" data-datafield="CONTRACT_DETAIL_ZY" data-type="SheetDetail" class="" style=""
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
                                            <select data-datafield="CONTRACT_DETAIL_ZY.FP_GROUP_ID" data-type="SheetDropDownList" id="ctl508406_control114" style="" data-onchange="setText(this,'CONTRACT_DETAIL_ZY.FP_GROUP_NAME')"
                                                 data-schemacode="M_financial_product_group" data-querycode="fun_M_financial_product_group" data-datavaluefield="FP_GROUP_ID" data-datatextfield="FP_GROUP_DSC" data-displayemptyitem="true">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title95" class="col-md-4">
                                            <span id="ctl508406_label95" style="">付款频率</span>
                                        </div>
                                        <div id="ctl508406_Data95" class="col-md-8">
                                            <select data-datafield="CONTRACT_DETAIL_ZY.FREQUENCY_CDE" data-type="SheetDropDownList" id="ctl508406_control95" style=""
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
                                            <select data-datafield="CONTRACT_DETAIL_ZY.FINANCIAL_PRODUCT_ID" data-type="SheetDropDownList" id="ctl508406_control4" style=""
                                                data-schemacode="M_FinancialType" data-querycode="fun_M_FinancialType" data-datavaluefield="financial_product_id" data-datatextfield="financial_product_dsc" data-displayemptyitem="true"
                                                data-filter="VEHICLE_DETAIL_ZY.CONDITION:asset_condition,APPLICATION_TYPE_CODE:type,bp_id:bp_id,CONTRACT_DETAIL_ZY.FP_GROUP_ID:group_id,VEHICLE_DETAIL_ZY.ASSET_MODEL_CDE:model_cde">
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
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.TOTAL_ASSET_COST" data-type="SheetTextBox" id="ctl508407_control27" style=""
                                                 data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title27" class="col-md-4">
                                            <span id="ctl508406_label27" style="">贷款期数（月）</span>
                                        </div>
                                        <div id="ctl508406_Data27" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.LEASE_TERM_IN_MONTH" data-type="SheetTextBox" id="ctl508406_control27" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title159" class="col-md-4">
                                            <span id="ctl508406_label159" style="">销售价格</span>
                                        </div>
                                        <div id="ctl508406_Data159" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.SALE_PRICE" data-type="SheetTextBox" disabled id="ctl508406_control159" style=""
                                                data-computationrule="2,{CONTRACT_DETAIL_ZY.TOTAL_ASSET_COST}+{CONTRACT_DETAIL_ZY.ACCESSORY_AMT}" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title10" class="col-md-4">
                                            <span id="ctl508406_label10" style="">合同价格</span>
                                        </div>
                                        <div id="ctl508406_Data10" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.ASSET_COST" data-type="SheetTextBox" disabled id="ctl508406_control10" style=""
                                                data-computationrule="2,{CONTRACT_DETAIL_ZY.TOTAL_ASSET_COST}+{CONTRACT_DETAIL_ZY.ACCESSORY_AMT}" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title75" class="col-md-4">
                                            <span id="ctl508406_label75" style="">首付款比例%</span>
                                        </div>
                                        <div id="ctl508406_Data75" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.SECURITY_DEPOSIT_PCT" data-type="SheetTextBox" id="ctl508406_control75" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title89" class="col-md-4">
                                            <span id="ctl508406_label89" style="">贷款额比例 %</span>
                                        </div>
                                        <div id="ctl508406_Data89" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.FINANCED_AMT_PCT" disabled data-type="SheetTextBox" id="ctl508406_control89" style=""
                                                >
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title23" class="col-md-4">
                                            <span id="ctl508406_label23" style="">资产残值/轻松融资尾款%</span>
                                        </div>
                                        <div id="ctl508406_Data23" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.BALLOON_PERCENTAGE" data-type="SheetTextBox" disabled id="ctl508406_control23" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title14" class="col-md-4">
                                            <span id="ctl508406_label14" style="">首付款金额</span>
                                        </div>
                                        <div id="ctl508406_Data14" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.CASH_DEPOSIT" data-type="SheetTextBox" id="ctl508406_control14" style="" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title21" class="col-md-4">
                                            <span id="ctl508406_label21" style="">贷款金额</span>
                                        </div>
                                        <div id="ctl508406_Data21" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.AMOUNT_FINANCED" disabled data-type="SheetTextBox" id="ctl508406_control21" style=""
                                                data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title25" class="col-md-4">
                                            <span id="ctl508406_label25" style="">资产残值/轻松融资尾款金额</span>
                                        </div>
                                        <div id="ctl508406_Data25" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.BALLOON_AMOUNT" data-type="SheetTextBox" disabled id="ctl508406_control25" style="" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title124" class="col-md-4">
                                            <span id="ctl508406_label124" style="">客户利率%</span>
                                        </div>
                                        <div id="ctl508406_Data124" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.BASE_CUSTOMER_RATE" disabled data-type="SheetTextBox" id="ctl508406_control124" style="" data-formatrule="{0:N4}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title141" class="col-md-4">
                                            <span id="ctl508406_label141" style="">实际利率%</span>
                                        </div>
                                        <div id="ctl508406_Data141" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.ACTUAL_RTE" disabled data-type="SheetTextBox" id="ctl508406_control141" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title139" class="col-md-4">
                                            <span id="ctl508406_label139" style="">贴息利率%</span>
                                        </div>
                                        <div id="ctl508406_Data139" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.CALC_SUBSIDY_RTE" disabled data-type="SheetTextBox" id="ctl508406_control139" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div id="ctl508407_Title124" class="col-md-4">
                                            <span id="ctl508407_label124" style="">应付总额</span>
                                        </div>
                                        <div id="ctl508407_Data124" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.YFJE" data-type="SheetTextBox" id="ctl508407_control124" style=""
                                                disabled data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title140" class="col-md-4">
                                            <span id="ctl508406_label140" style="">贴息金额</span>
                                        </div>
                                        <div id="ctl508406_Data140" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.CALC_SUBSIDY_AMT" disabled data-type="SheetTextBox" id="ctl508406_control140" style="" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title62" class="col-md-4">
                                            <span id="ctl508406_label62" style="">利息总额</span>
                                        </div>
                                        <div id="ctl508406_Data62" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.ASSETITC" disabled data-type="SheetTextBox" id="ctl508406_control62" style="" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title1" class="col-md-4">
                                            <span id="ctl508406_label1" style="">IDENTIFICATION_CODE</span>
                                        </div>
                                        <div id="ctl508406_Data1" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.IDENTIFICATION_CODE8" data-type="SheetTextBox" id="ctl508406_control1" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl508407_Title126" class="col-md-4">
                                            <span id="ctl508407_label126" style="">总附加费价格</span>
                                        </div>
                                        <div id="ctl508407_Data126" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.TOTAL_ACCESSORY_AMT" data-type="SheetTextBox" id="ctl508407_control126" style=""
                                                data-computationrule="2,{CONTRACT_DETAIL_ZY.ACCESSORY_AMT}" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div class="col-md-4">
                                            <span>产品类型名称</span>
                                        </div>
                                        <div class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.FINANCIAL_PRODUCT_NAME" data-type="SheetTextBox" id="ctl50406_control1" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="col-md-4">
                                            <span style="">产品组名称</span>
                                        </div>
                                        <div class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.FP_GROUP_NAME" data-type="SheetTextBox" id="ctl50407_control126" style="" />
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
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.WCYE" data-type="SheetTextBox" id="ctl508407_control62" style=""
                                                disabled data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl508406_Title157" class="col-md-4">
                                            <span id="ctl508406_label157" style="">展期期数</span>
                                        </div>
                                        <div id="ctl508406_Data157" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.DEFERRED_TRM" disabled data-type="SheetTextBox" id="ctl508406_control157" style="">
                                        </div>
                                    </div>
                                    <%--<div class="col-md-4">
                                        <div id="ctl508406_Title162" class="col-md-4">
                                            <span id="ctl508406_label162" style="">选装部件</span>
                                        </div>
                                        <div id="ctl508406_Data162" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.ACCESSORIES_IND" data-type="SheetTextBox" id="ctl508406_control162" style="">
                                        </div>
                                    </div>--%>
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title126" class="col-md-4">
                                            <span id="ctl508406_label126" style="">附加费价格</span>
                                        </div>
                                        <div id="ctl508406_Data126" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL_ZY.ACCESSORY_AMT" disabled data-type="SheetTextBox" id="ctl508406_control126" style=""
                                                data-computationrule="2,SUM({ASSET_ACCESSORY_ZY.PRICE})" data-formatrule="{0:N2}">
                                        </div>
                                    </div>
                                    <div class="col-md-4 hidden">
                                        <div id="ctl508406_Title158" class="col-md-4">
                                            <span id="ctl508406_label158" style="">借款用途</span>
                                        </div>
                                        <div id="ctl508406_Data158" class="col-md-8">
                                            <select><option value="0" selected>自购用车</option></select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>



        <%-- 简版还款计划表 --%>
        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <span>还款计划</span>
        </div>
        <div class="row tableContent bottom">
            <div id="div801525" class="col-md-12">
                <table id="ctl620410" data-datafield="PMS_RENTAL_DETAIL_ZY" data-type="SheetGridView" class="SheetGridView" style=""
                    data-defaultrowcount="0" data-displayadd="false">
                    <tbody>
                        <tr class="header">
                            <td class="rowSerialNo hidden">序号</td>
                            <td style="" class="hidden">RENTAL_DETAIL_SEQ</td>
                            <td style="">还款起始期</td>
                            <td style="">还款结束期</td>
                            <td style="">还款额</td>
                            <td style="">每期还款总额</td>
                            <td style="" class="hidden">INTEREST_RTE</td>
                        </tr>
                        <tr class="template">
                            <td class="hidden"></td>
                            <td id="ctl620410_td1" data-datafield="PMS_RENTAL_DETAIL_ZY.RENTAL_DETAIL_SEQ" style="" class="hidden">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL_ZY.RENTAL_DETAIL_SEQ" data-type="SheetTextBox" id="ctl620410_control1" style=""></td>
                            <td id="ctl620410_td3" data-datafield="PMS_RENTAL_DETAIL_ZY.START_TRM" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL_ZY.START_TRM" data-type="SheetTextBox" id="ctl620410_control3" style="text-align: center"></td>
                            <td id="ctl620410_td4" data-datafield="PMS_RENTAL_DETAIL_ZY.END_TRM" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL_ZY.END_TRM" data-type="SheetTextBox" id="ctl620410_control4" style="text-align: center"></td>
                            <td id="ctl620410_td5" data-datafield="PMS_RENTAL_DETAIL_ZY.RENTAL_AMT" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL_ZY.RENTAL_AMT" data-type="SheetTextBox" id="ctl620410_control5" style="text-align: center" data-formatrule="{0:N2}"></td>
                            <td id="ctl620410_td6" data-datafield="PMS_RENTAL_DETAIL_ZY.EQUAL_RENTAL_AMT" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL_ZY.EQUAL_RENTAL_AMT" data-type="SheetTextBox" id="ctl620410_control6" style="text-align: center" data-formatrule="{0:N2}"></td>
                            <td id="ctl620410_td7" data-datafield="PMS_RENTAL_DETAIL_ZY.INTEREST_RTE" style="" class="hidden">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL_ZY.INTEREST_RTE" data-type="SheetTextBox" id="ctl620410_control7" style=""></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <%-- 借款码 --%>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divattachment',this)">
            <label id="Label1" data-en_us="Sheet information">借款码</label>
        </div>
        <div class="divContent" id="divattachment">
            <div class="row">
                <div id="title363" class="col-md-2">
                    <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">借款码</span>
                </div>
                <div id="control363" class="col-md-10">
                    <div id="Control343" class="hidden" data-datafield="zy_contractQrCode" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                    </div>

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


        $.MvcSheet.PreInit = function () {
            $.MvcSheetUI.SheetInfo.PermittedActions.Save = false;            $.MvcSheetUI.SheetInfo.PermittedActions.ViewInstance = true;            $.MvcSheetUI.SheetInfo.PermittedActions.Submit = false;            $.MvcSheetUI.SheetInfo.PermittedActions.Print = true;
            $.MvcSheetUI.SheetInfo.PermittedActions.Reject = false;
            $.MvcSheetUI.SheetInfo.PermittedActions.Forward = false;
            $.MvcSheetUI.SheetInfo.PermittedActions.Close = true;
        }

        // 页面加载完成事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            getApplicantUserInfoByBizObject();
            
            hideFIrow();
            getmsg();

            var imageItem = $("#Control343").find(".fa-download");

            $("#control363").append("<img data-original=\"" + imageItem[0].href + "\" src=\"" +  imageItem[0].href + "\" ></img>");

            document.oncontextmenu = function () {
                return false;
            }
        } 

        
    </script>
</asp:Content>
        