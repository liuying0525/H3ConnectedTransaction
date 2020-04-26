<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FKSQ.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.FKSQ" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="/Portal/jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <link type="text/css" href="css/common.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" rel="stylesheet" />
    <script src="/Portal/jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
    <script src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" type="text/javascript"></script>
    <script src="js/Validate.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" type="text/javascript"></script>
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
    <script src="/Portal/WFRes/_Scripts/TemplatePrint.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <script type="text/javascript" src="/Portal/Custom/js/ocr.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">个人汽车贷款申请</label>
        <label id="lblFaceSign" class="panel-title hidden" style="margin-left:50px;color:red">附加视频面核</label>
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
            <div class="row">
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
                    <div style="padding: 5px 0;">
                        <textarea id="addmsg"></textarea>
                        <br />
                        <a href="javascript:void(0);" id="addmsga">提交</a>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-right bannerTitle" id="titleAsset" onclick="hidediv('div_Detail_VEHICLE',this)">
            <span>资产信息</span>
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
                                    <div class="col-md-4">
                                        <div id="ctl524045_Title3" class="col-md-4">
                                            <span id="ctl524045_label3" style="">制造商Code</span>
                                        </div>
                                        <div id="ctl524045_Data3" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MAKE_CDE" data-type="SheetTextBox" id="ctl524045_control3" style="" />
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524045_Title75" class="col-md-4">
                                            <span id="ctl524045_label75" style="">车型Code</span>
                                        </div>
                                        <div id="ctl524045_Data75" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_BRAND_CDE" data-type="SheetTextBox" id="ctl524045_control75" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title71" class="col-md-4">
                                            <span id="ctl524044_label71" style="">VEHICLE_TYPE_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data71" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_TYPE_CDE" data-type="SheetTextBox" id="ctl524044_control71" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title72" class="col-md-4">
                                            <span id="ctl524044_label72" style="">VEHICLE_SUBTYPE_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data72" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_SUBTYPE_CDE" data-type="SheetTextBox" id="ctl524044_control72" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title4" class="col-md-4">
                                            <span id="ctl524044_label4" data-type="SheetLabel" style="">ASSET_MODEL_CDE</span>
                                        </div>
                                        <div id="ctl524044_Data4" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MODEL_CDE" data-type="SheetTextBox" id="ctl524044_control4" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title78" class="col-md-4">
                                            <span id="ctl524044_label78" style="">资产ID</span>
                                        </div>
                                        <div id="ctl524044_Data78" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_ID" data-type="SheetTextBox" id="ctl524044_control78" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl524044_Title80" class="col-md-4">
                                            <span id="ctl524044_label80" style="">MIOCN_DSC</span>
                                        </div>
                                        <div id="ctl524044_Data80" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_DSC" data-type="SheetTextBox" id="ctl524044_control80" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div id="ctl5240414_Title83" class="col-md-4">
                                            <span id="ctl5240414_label83" style="">担保方式</span>
                                        </div>
                                        <div id="ctl5240414_Data83" class="col-md-8">
                                            <input type="text" data-datafield="VEHICLE_DETAIL.GURANTEE_OPTION" data-type="SheetTextBox" id="ctl8441175" style="">
                                        </div>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-12">
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
                <div class="col-md-4">
                    <div class="col-md-4">
                        <span data-type="SheetLabel" data-datafield="Bankname" style="">发动机号码</span>
                    </div>
                    <div class="col-md-8">
                        <input type="text" data-datafield="engine_number" data-type="SheetTextBox" id="ctl_engine_number" class="" style="" 
                            onkeyup="this.value=strMaxLength(this.value,'40')"/>
                    </div>
                </div>
                <div class="col-md-8">
                    <div class="col-md-4" style="width:16.67%">
                        <span data-type="SheetLabel" data-datafield="Branchname" style="">VIN码</span>
                    </div>
                    <div class="col-md-8">
                        <input type="text" data-datafield="vin_number" data-type="SheetTextBox" id="ctl_vin_number" class="" style="" 
                            data-onchange="vin_change(this)" maxlength="17"/>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <div id="title461" class="col-md-4">
                        <span id="Label413" data-type="SheetLabel" data-datafield="Bankname" style="">扣款银行</span>
                    </div>
                    <div id="control461" class="col-md-8">
                        <select onchange="validate_bank(); TemplateChange()" data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-displayemptyitem="true" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
                    </div>
                </div>
                <div class="col-md-8">
                    <div id="title462" class="col-md-4" style="width:16.67%">
                        <span id="Label414" data-type="SheetLabel" data-datafield="Branchname" style="">银行分支</span>
                    </div>
                    <div id="control462" class="col-md-8">
                        <select data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-displayemptyitem="true" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <div id="title463" class="col-md-4">
                        <span id="Label415" data-type="SheetLabel" data-datafield="Accoutname" style="">账户名</span>
                    </div>
                    <div id="control463" class="col-md-8">
                        <select id="Control415s" style="display: none;" onchange="chkzhm(this)"></select>
                        <input id="Control415" type="text" data-datafield="Accoutname" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="col-md-4">
                    <div id="title464" class="col-md-4">
                        <span id="Label416" data-type="SheetLabel" data-datafield="AccoutNumber" style="">账户号</span>
                    </div>
                    <div id="control464" class="col-md-8">
                        <input id="Control416" type="text" oninput="textnum(this)" data-datafield="AccoutNumber" data-type="SheetTextBox" style="" maxlength="20">
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="col-md-4">
                        <button type="button" id="btn_update_to_cap" onclick="update_to_cap('true')" class="btn btn-default" style="line-height: 20px;">更新到CAP</button>
                    </div>
                    <div class="col-md-8">
                        <span><span style="color:red">Tips</span>&nbsp;更新之后再去打印合同，没有的话可直接打印合同</span>
                    </div>
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
        <div class="nav-icon fa  fa-chevron-right bannerTitle" id="titleJRTK" onclick="hidediv('div_Detail_CONTRACT',this)">
            <span>金融条款</span>
        </div>
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
                                    <div class="col-md-4">
                                        <div id="ctl508406_Title1" class="col-md-4">
                                            <span id="ctl508406_label1" style="">IDENTIFICATION_CODE</span>
                                        </div>
                                        <div id="ctl508406_Data1" class="col-md-8">
                                            <input type="text" data-datafield="CONTRACT_DETAIL.IDENTIFICATION_CODE8" data-type="SheetTextBox" id="ctl508406_control1" style="">
                                        </div>
                                    </div>
                                    <div class="col-md-4">
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
        <%-- 简版还款计划表 --%>
        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <span>还款计划</span>
            <button type="button" class="btn btn-primary btn-md" data-toggle="modal" data-target="#pmt_detail" style="margin: 3px; float: right; margin-left: 10px">详细还款计划</button>
        </div>
        <div class="row tableContent bottom">
            <div id="div801525" class="col-md-12">
                <table id="ctl620410" data-datafield="PMS_RENTAL_DETAIL" data-type="SheetGridView" class="SheetGridView" style=""
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
                            <td id="ctl620410_td1" data-datafield="PMS_RENTAL_DETAIL.RENTAL_DETAIL_SEQ" style="" class="hidden">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.RENTAL_DETAIL_SEQ" data-type="SheetTextBox" id="ctl620410_control1" style=""></td>
                            <td id="ctl620410_td3" data-datafield="PMS_RENTAL_DETAIL.START_TRM" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.START_TRM" data-type="SheetTextBox" id="ctl620410_control3" style="text-align: center"></td>
                            <td id="ctl620410_td4" data-datafield="PMS_RENTAL_DETAIL.END_TRM" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.END_TRM" data-type="SheetTextBox" id="ctl620410_control4" style="text-align: center"></td>
                            <td id="ctl620410_td5" data-datafield="PMS_RENTAL_DETAIL.RENTAL_AMT" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.RENTAL_AMT" data-type="SheetTextBox" id="ctl620410_control5" style="text-align: center" data-formatrule="{0:N2}"></td>
                            <td id="ctl620410_td6" data-datafield="PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT" style="">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT" data-type="SheetTextBox" id="ctl620410_control6" style="text-align: center" data-formatrule="{0:N2}"></td>
                            <td id="ctl620410_td7" data-datafield="PMS_RENTAL_DETAIL.INTEREST_RTE" style="" class="hidden">
                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.INTEREST_RTE" data-type="SheetTextBox" id="ctl620410_control7" style=""></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <%-- 还款计划表明细 --%>
        <!-- Modal -->
        <div class="modal fade" id="pmt_detail" tabindex="-1" role="dialog" aria-labelledby="myModalLabel_pmt">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <input type="text" id="hide_plan_id" class="hidden"/>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel_pmt">还款计划明细</h4>
                    </div>
                    <div class="modal-body">
                        <div style="margin:10px 27px -11px 10px">
                            <table class="table table-hover table-bordered">
                                <thead>
                                    <tr>
                                        <th>期数</th>
                                        <th>未偿本金</th>
                                        <th>还款额</th>
                                        <th>本金总额</th>
                                        <th>利息</th>
                                        <th>每期还款总额</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                        <div style="margin:10px;max-height:350px;overflow-y:auto;overflow-x:hidden" class="table_body mycontent"></div>
                        <div style="margin:-11px 27px 10px 10px" class="table_foot mycontent"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" id="btn_plan_detail_print" onclick="plan_detail_print()">打印</button> 
                        <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" id="divattachmentinfo" onclick="hidediv('divattachment',this)">
            <label id="Label1" data-en_us="Sheet information">附件信息</label>
            <a href="javascript:void(0);" onclick="getDownLoadURL()">附件详情</a>
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
                    <select name="select2" id="select2" style="width: 200px" onchange="validate_bank(); TemplateChange()">
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
            getApplicantUserInfoByBizObject();
            var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
            var IsWork = $.MvcSheetUI.QueryString("Mode");
            //关于打印加载事件
            SheetLoaded();
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
            if (IsMobile) {
                $("#ctl64724").parent().addClass("chkbank");
                $("#ctl875733").parent().addClass("chkbank");
            }
            hideFIrow();
            getmsg();

            var needSign = $.MvcSheetUI.SheetInfo.BizObject.DataItems["NeedFaceSign"].V; 
            if (needSign == "1") {
                $("#lblFaceSign").removeClass("hidden");
            }

            //风控审核结果转换
            var sysType = $.MvcSheetUI.SheetInfo.BizObject.DataItems["FK_SYS_TYPE"].V; 

            if (sysType == "1") {
                var fkresult = $.MvcSheetUI.SheetInfo.BizObject.DataItems["FK_RESULT"].V; 

                if (fkresult == null || fkresult == "" || fkresult == undefined) {
                    fkresult = "";
                }

                $("#Div3").append("<span style='color:red'>" + fkresult + "</span>");
            }
            else {
                var fkresult = $.MvcSheetUI.SheetInfo.BizObject.DataItems["fkResult"].V; 
                var arrrsrq = []
                arrrsrq['localreject'] = "东正本地规则<span style=\"color:red;\">拒绝</span>";
                arrrsrq['cloudaccept'] = "云端规则<span style=\"color:red;\">通过</span>";
                arrrsrq['cloudreject'] = "云端规则<span style=\"color:red;\">拒绝</span>";
                arrrsrq['cloudmanual'] = "云端规则返回<span style=\"color:red;\">转人工</span>";
                arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
                $("#Div3").append(arrrsrq[fkresult]);
            }

            //添加留言
            $('#addmsga').on('click', function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
            $('#pmt_detail').on('shown.bs.modal', function (e) {
                var v_hide_plan_id = $("#hide_plan_id").val();


                var plan_id = $.MvcSheetUI.GetControlValue("PLAN_ID");
                if (plan_id == "") {
                    $('#pmt_detail .modal-body div.mycontent').empty();//清空
                    $('#pmt_detail .modal-body div.table_body').append("<h1>还款计划ID为空</h1>");
                }
                else {
                    if (v_hide_plan_id == plan_id) {//没有修改过Plan id不用重新取值；

                    }
                    else { //修改过Plan_ID，需要重新取值
                        $("#hide_plan_id").val(plan_id);
                        $('#pmt_detail .modal-body div.mycontent').empty();//清空
                        var rows = get_PMT_Detail(plan_id);
                        var table_rows = "";
                        var hk_total = 0;//还款总额
                        var bj_total = 0;//本金总额
                        var lx_total = 0;//利息总额
                        var convert_his = {};//缓存，减少计算的次数，提高速度 ；
                        rows.forEach(function (row, num) {
                            var pmt_str
                            if (convert_his[row.PMT]) {
                                pmt_str = convert_his[row.PMT];
                            }
                            else {
                                pmt_str = calculate("add", row.PMT, 0, "N2");
                            }

                            table_rows += "<tr><td>" + row.TERM_ID + "</td><td>" + calculate("add", row.TOTAL_RECEIVE, 0, "N2") + "</td><td>"
                                + pmt_str + "</td><td>" + calculate("add", row.PRINCIPAL, 0, "N2") + "</td><td>" + calculate("add", row.ASSETITC, 0, "N2") + "</td><td>" + pmt_str + "</td></tr>";
                            hk_total = calculate("add", row.PMT, hk_total);
                            bj_total = calculate("add", row.PRINCIPAL, bj_total);
                            lx_total = calculate("add", row.ASSETITC, lx_total);
                        });
                        var hk_total_str = calculate("add", hk_total, 0, "N2");
                        var table_body = "<table class=\"table table-hover table-bordered\"><tbody>" + table_rows + "</tbody></table>";
                        var table_foot = "<table class=\"table table-hover table-bordered\"><tfoot><tr><th>" + rows.length + "期</th><th></th><th>" + hk_total_str + "</th><th>" +
                            calculate("add", bj_total, 0, "N2") + "</th><th>" + calculate("add", lx_total, 0, "N2") + "</th><th>" + hk_total_str + "</th></tr></tfoot></table>";
                        $('#pmt_detail .modal-body div.table_body').append(table_body);
                        $('#pmt_detail .modal-body div.table_foot').append(table_foot);
                    }
                }
            });

            var condition = getCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, "Condition");
            if (condition == "U" && $.MvcSheetUI.GetControlValue("vin_number") != "") {//二手车不可编辑
                $.MvcSheetUI.GetElement("engine_number").attr("disabled", "disabled");
                $.MvcSheetUI.GetElement("vin_number").attr("disabled", "disabled");
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
            var result = true;
            if (this.Action == "Submit") {
                result = vin_change($("#ctl_vin_number").get(0));;
                if (!result)
                    return false;
                result = validate_data_to_cap();
            }
            return result
        }

        function update_to_cap(need_validate) {
            if (need_validate){
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
                
        // 验证扣款银行和打印模板 wangxg 19.8
        // if 打印模板 = 建设银行客户扣款授权书(个人版) then 扣款银行必须是建设银行
        // if 打印模板 = 工商银行客户扣款授权书(个人版) then 扣款银行必须是工商银行
        // if 扣款银行 = 招商银行 then 打印模板 不能选择 工行 和 建行
        function validate_bank() {
            var tmp = $("#select2").find("option:selected").text();
            var bankName = $.MvcSheetUI.GetControlValue("Bankname");
            if (bankName === '招商银行股份有限公司' &&
                (tmp === '工商银行客户扣款授权书(个人版)' || tmp === '建设银行客户扣款授权书(个人版)')) {
                $("#select2").get(0).selectedIndex = 0;
                alert('扣款银行是招商银行，不能选择此打印模板');
                return 0;
            } else if (bankName === '' && (tmp === '工商银行客户扣款授权书(个人版)' || tmp === '建设银行客户扣款授权书(个人版)')) {
                 $("#select2").get(0).selectedIndex = 0;
                alert('请先选择扣款银行');
                return 0;
            } else if (tmp === '建设银行客户扣款授权书(个人版)' && bankName !== '中国建设银行股份有限公司') {
                $("#select2").get(0).selectedIndex = 0;
                alert('请选择正确的扣款授权书模板');
                return 0;
            } else if (tmp === '工商银行客户扣款授权书(个人版)' && bankName !== '中国工商银行') {
                $("#select2").get(0).selectedIndex = 0;
                alert('请选择正确的扣款授权书模板');
                return 0;
            }
            //debugger;
            return 1;
        }
    </script>
</asp:Content>
