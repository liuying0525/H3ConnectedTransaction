<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FaceSignVerify.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.FaceSignVerify" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

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
    <script src="js/FaceSignVerify.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" type="text/javascript"></script>
    <script type="text/javascript" src="/Portal/WFRes/layer/layer.js"></script>
        <script type="text/javascript">
        var videourl = '<%=ConfigurationManager.AppSettings["facesignvideourl"] + string.Empty%>';
    </script>
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
        <label id="lblTitle" class="panel-title">面签人工审核</label>
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
                    <div style="padding: 5px 0;">
                        <textarea id="addmsg"></textarea>
                        <br />
                        <a href="javascript:void(0);" id="addmsga">提交</a>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="nav-icon fa  fa-chevron-right bannerTitle" id="titleAsset" onclick="hidediv('div_FaceSignInfo',this)">
            <span>人员面签信息</span>
        </div>
        <div id="div_FaceSignInfo" style="padding-right: 0%">

        </div>


        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" onclick="videoStop()" aria-hidden="true">
                            &times;
                        </button>
                        <h4 class="modal-title" id="myModalLabel">面签视频</h4>
                    </div>
                    <div class="modal-body" style="text-align:center">
                        <video id='facevideo' src="" controls="controls" controlslist="nodownload" type="video/mp4"></video>
                    </div>
                    <div class="modal-footer">
                    </div>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
