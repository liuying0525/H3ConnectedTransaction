<%@ Page Language="C#" AutoEventWireup="true" CodeFile="YYFKSQ.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.FKSQ" EnableEventValidation="false" MasterPageFile="~/MvcSheetYYGD.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="/Portal/jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <link type="text/css" href="css/common.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" rel="stylesheet" />
    <script src="/Portal/jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Portal/WFRes/layer/layer.js?20180712"></script>
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

        #divBaseInfo div {
            text-align: center;
        }
    </style>
    <script src="/Portal/WFRes/_Scripts/TemplatePrint.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <div class="panel-body sheetContainer" style="padding: 0;">
        <div class="nav-icon bannerTitle">
            <label data-en_us="Basic information">申请信息</label>
            <div style="float: right; font-weight: 300; font-size: 14px; padding-right: 5%; cursor: pointer;" onclick="showliuyan()">
                <img src="/Portal/img/Images/liuyan.png" style="position: relative; top: -4px;" />
                <span>留言</span>
                <a id="lysq">收起 &and;</a>
            </div>
        </div>
        <div class="divContent" id="divBaseInfo">
            <div class="row tableContent">
                <div class="row tableContent" style="border: none; padding-bottom: 30px;" id="showliuyan">
                    <div id="control445" class="col-md-12">
                        <table class="SheetGridView" style="text-align: center; width: 100%;">
                            <tbody id="liuyan">
                                <tr class="header">
                                    <td class="rowSerialNo">序号</td>
                                    <td>
                                        <label>留言内容</label>
                                    </td>
                                    <td style="width: 80px;">
                                        <label>留言人员</label>
                                    </td>
                                    <td style="width: 125px;">
                                        <label>留言时间</label>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div style="padding: 5px 0;">
                            <textarea id="addmsg"></textarea>
                            <a href="javascript:void(0);" id="addmsga">提交</a>
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div class="row hidden">
                    <div id="title40000" class="col-md-2">
                        bp_id
                    </div>
                    <div id="control40000" class="col-md-4">
                        <input type="text" id="hidden_001" data-datafield="bp_id" data-type="SheetTextBox" style="" />
                    </div>
                </div>
                <div class="row tableContent hidden">
                    <div class="col-md-12">
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
                                            <div id="ctl508407_Title124" class="col-md-4">
                                                <span id="ctl508407_label124" style="">应付总额</span>
                                            </div>
                                            <div id="ctl508407_Data124" class="col-md-8">
                                                <input type="text" data-datafield="CONTRACT_DETAIL.YFJE" data-type="SheetTextBox" id="ctl508407_control124" style=""
                                                    disabled data-formatrule="{0:F2}">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
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
                <div class="row tableContent hidden">
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
                                    <div class="row">
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title22" class="col-md-4">
                                                <span id="ctl524044_label22" style="">变速器</span>
                                            </div>
                                            <div id="ctl524044_Data22" class="col-md-8">
                                                <select data-datafield="VEHICLE_DETAIL.TRANSMISSION" data-type="SheetDropDownList" id="ctl524044_control22" style=""
                                                    data-masterdatacategory="变速器" data-displayemptyitem="true" data-queryable="false">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title51" class="col-md-4">
                                                <span id="ctl524044_label51" style="">车身颜色</span>
                                            </div>
                                            <div id="ctl524044_Data51" class="col-md-8">
                                                <select data-datafield="VEHICLE_DETAIL.COLOR" disabled data-type="SheetDropDownList" id="ctl524044_control51" style="" data-queryable="false"
                                                    data-schemacode="M_asset_color" data-querycode="fun_M_asset_color" data-datavaluefield="COLOR_CDE" data-datatextfield="COLOR_DSC" data-displayemptyitem="true">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title24" class="col-md-4">
                                                <span id="ctl524044_label24" style="">资产价格</span>
                                            </div>
                                            <div id="ctl524044_Data24" class="col-md-8">
                                                <input type="text" data-datafield="VEHICLE_DETAIL.NEW_PRICE" readonly data-type="SheetTextBox" id="ctl524044_control24" style="" data-formatrule="{0:F2}">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title83" class="col-md-4">
                                                <span id="ctl524044_label83" style="">担保方式</span>
                                            </div>
                                            <div id="ctl524044_Data83" class="col-md-8">
                                                <%-- <textarea data-datafield="VEHICLE_DETAIL.GURANTEE_OPTION" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl524044_control83">											</textarea>--%>
                                                <div data-datafield="VEHICLE_DETAIL.GURANTEE_OPTION_H3" data-type="SheetCheckboxList" id="ctl844175" class="" style="" data-repeatdirection="Vertical" data-repeatcolumns="1" data-masterdatacategory="担保方式" data-onchange="gurantee_option_chg()"></div>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title19" class="col-md-4">
                                                <span id="ctl524044_label19" style="">发动机号码</span>
                                            </div>
                                            <div id="ctl524044_Data19" class="col-md-8">
                                                <input type="text" data-datafield="VEHICLE_DETAIL.ENGINE" data-type="SheetTextBox" id="ctl524044_control19" style="" onkeyup="this.value=strMaxLength(this.value,'40')">
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title28" class="col-md-4">
                                                <span id="ctl524044_label28" style="">VIN码</span>
                                            </div>
                                            <div id="ctl524044_Data28" class="col-md-8">
                                                <input type="text" data-datafield="VEHICLE_DETAIL.VIN_NUMBER" readonly data-type="SheetTextBox" maxlength="17" id="ctl524044_control28"
                                                    style="" data-onchange="vin_change(this)" onkeyup="this.value=strMaxLength(this.value,'25')">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row hidden">
                                        <div class="col-md-4">
                                            <div id="ctl524044_Title2" class="col-md-4">
                                                <span id="ctl524044_label2" style="">IDENTIFICATION_CODE</span>
                                            </div>
                                            <div id="ctl524044_Data2" class="col-md-8">
                                                <input type="text" data-datafield="VEHICLE_DETAIL.IDENTIFICATION_CODE7" data-type="SheetTextBox" id="ctl524044_control2" style="">
                                            </div>
                                        </div>
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
                    <div id="title14" class="col-md-2">
                        <span id="Label24" data-type="SheetLabel" data-datafield="BUSINESS_PARTNER_ID" style="">经销商</span>
                    </div>
                    <div id="control1019" class="col-md-4">
                        <select id="Control24" data-datafield="BUSINESS_PARTNER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                            data-schemacode="M_business_partner_id" data-querycode="003" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                        </select>
                    </div>
                    <div id="title1" class="col-md-2">
                        <span id="Label11" data-type="SheetLabel" data-datafield="APPLICATION_NUMBER" style="">贷款申请号码</span>
                    </div>
                    <div id="control1" class="col-md-4">
                        <input id="Control11" type="text" data-datafield="APPLICATION_NUMBER" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title1411" class="col-md-2">
                        <span id="Label2411" data-type="SheetLabel" data-datafield="jxskhh" style="">经销商开户行</span>
                    </div>
                    <div id="control101911" class="col-md-4">
                        <label>
                            <span id="span_jxskhh" data-type="SheetLabel" style=""></span>
                        </label>
                    </div>
                    <div id="title1111" class="col-md-2">
                        <span id="Label1111" data-type="SheetLabel" data-datafield="jxskhhkh" style="">开户行卡号</span>
                    </div>
                    <div id="control111" class="col-md-4">
                        <label>
                            <span id="span_jxskhhkh" data-type="SheetLabel" style=""></span>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div id="Div8" class="col-md-2">
                        <label>
                            <span id="Span2" data-type="SheetLabel" data-datafield="jxsed" style="">是否抵押</span>
                        </label>
                    </div>
                    <div id="Div9" class="col-md-4">
                        <label>
                            <span id="span_sfdy" data-type="SheetLabel" style=""></span>
                        </label>
                    </div>
                    <div id="title141" class="col-md-2">
                        <label>
                            <span id="Label241" data-type="SheetLabel" data-datafield="jxsed" style="">经销商额度</span>
                        </label>
                    </div>
                    <div id="control141" class="col-md-4">
                        <label>
                            <span id="span_jxsed" data-type="SheetLabel" style=""></span>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div id="Div10" class="col-md-2">
                        <label>
                            <span id="Span4" data-type="SheetLabel" data-datafield="kyed" style="">已使用额度</span>
                        </label>
                    </div>
                    <div id="Div11" class="col-md-4">
                        <label>
                            <span id="span_ysyed" data-type="SheetLabel" style=""></span>
                        </label>
                    </div>
                    <div id="title111" class="col-md-2">
                        <label>
                            <span id="Label111" data-type="SheetLabel" data-datafield="kyed" style="">可用额度</span>
                        </label>
                    </div>
                    <div id="Div7" class="col-md-4">
                        <label>
                            <span id="span_kyed" data-type="SheetLabel" style=""></span>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div id="divOriginateOUNameTitle" class="col-md-2">
                        <label id="lblOriginateOUNameTitle" data-type="SheetLabel" data-datafield="Originator.OUName" data-en_us="Originate OUName" data-bindtype="OnlyVisiable" style="">所属组织</label>
                    </div>
                    <div id="divOriginateOUName" class="col-md-4">
                        <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style="">
                        </select>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">资产状况</label>
                    </div>
                    <div class="col-md-4" id="asset_status">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">新车指导价</label>
                    </div>
                    <div class="col-md-4" id="asset_price">
                        <label><span></span></label>
                    </div>
                    <%--<div class="col-md-2">
                        <label data-type="SheetLabel" style="">VIN号</label>
                    </div>
                    <div class="col-md-4" id="asset_vinNo">
                        <label><span></span></label>
                    </div>--%>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">VIN号</label>
                    </div>
                    <div class="col-md-4">
                        <input type="text" data-datafield="vin_number" data-type="SheetTextBox" id="ctl_vin_number" class="" style="" 
                            maxlength="17"/>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">动力参数</label>
                    </div>
                    <div class="col-md-4" id="powerPara">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">备注</label>
                    </div>
                    <div class="col-md-4" id="modelRemark">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">贷款期数（月）</label>
                    </div>
                    <div class="col-md-4" id="asset_term">
                        <label><span></span></label>
                    </div>
                    <div id="title461" class="col-md-2">
                        <span id="Label413" data-type="SheetLabel" data-datafield="Bankname" style="">扣款银行</span>
                    </div>
                    <div id="control461" class="col-md-4">
                        <select data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-displayemptyitem="true" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
                    </div>
                </div>
                <div class="row">
                    <div id="title463" class="col-md-2">
                        <span id="Label415" data-type="SheetLabel" data-datafield="Accoutname" style="">账户名</span>
                    </div>
                    <div id="control463" class="col-md-4">
                        <select id="Control415s" style="display: none;" onchange="chkzhm(this)"></select>
                        <input id="Control415" type="text" data-datafield="Accoutname" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title464" class="col-md-2">
                        <span id="Label416" data-type="SheetLabel" data-datafield="AccoutNumber" style="">账户号</span>
                    </div>
                    <div id="control464" class="col-md-4">
                        <input id="Control416" type="text" oninput="textnum(this)" data-datafield="AccoutNumber" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">资产价格</label>
                    </div>
                    <div class="col-md-4" id="asset_sale_price">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">应付余额</label>
                    </div>
                    <div class="col-md-4" id="asset_yfye">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">首付款比例</label>
                    </div>
                    <div class="col-md-4" id="asset_sfkbl">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">首付款金额</label>
                    </div>
                    <div class="col-md-4" id="asset_sfkje">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">贷款额比例%</label>
                    </div>
                    <div class="col-md-4" id="asset_dkebl">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">贷款金额</label>
                    </div>
                    <div class="col-md-4" id="asset_dkje">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">尾款比例%</label>
                    </div>
                    <div class="col-md-4" id="asset_wkbl">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">尾款金额</label>
                    </div>
                    <div class="col-md-4" id="asset_wkje">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">实际利率</label>
                    </div>
                    <div class="col-md-4" id="asset_sjll">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">贴息利率</label>
                    </div>
                    <div class="col-md-4" id="asset_txll">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">客户利率</label>
                    </div>
                    <div class="col-md-4" id="asset_khll">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">贴息金额</label>
                    </div>
                    <div class="col-md-4" id="asset_txje">
                        <label><span></span></label>
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
                    <div id="title42110" class="col-md-2">
                        <span id="Label37910" data-type="SheetLabel" data-datafield="MQJG" style="">面签结果</span>
                    </div>
                    <div id="Div310" class="col-md-4">
                        <div data-datafield="MQJG" data-type="SheetRadioButtonList" id="ctl757048" class="" style="" data-defaultitems="大于70分;少于等于70分" data-defaultselected="false" data-repeatcolumns="2"></div>
                    </div>
                </div>
            </div>
            <div id="sqxx">
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
                    <div id="divSequenceNoTitle" class="col-md-2">
                        <label id="lblSequenceNoTitle" data-type="SheetLabel" data-datafield="SequenceNo" data-en_us="SequenceNo" data-bindtype="OnlyVisiable" style="">流水号</label>
                    </div>
                    <div id="divSequenceNo" class="col-md-4">
                        <label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style="">
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div id="title2" class="col-md-2">
                        <span id="Label12" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_CODE" style="">申请类型</span>
                    </div>
                    <div id="control2" class="col-md-4">
                        <select id="Control58" data-datafield="APPLICATION_TYPE_CODE" data-type="SheetDropDownList" style="" data-masterdatacategory="申请类型" data-queryable="false"></select>
                    </div>
                </div>
                <div class="row">
                    <div id="title4" class="col-md-2">
                        <span id="Label14" data-type="SheetLabel" data-datafield="BP_COMPANY_ID" style="">申请贷款公司</span>
                    </div>
                    <div id="control4" class="col-md-4">
                        <select id="Text1" data-datafield="BP_COMPANY_ID" data-type="SheetDropDownList" data-masterdatacategory="申请贷款公司" style="" data-queryable="false"></select>
                    </div>

                    <div id="title13" class="col-md-2">
                        <span id="Label23" data-type="SheetLabel" data-datafield="REFERENCE_NBR" style="">申请参考号</span>
                    </div>
                    <div id="control13" class="col-md-4">
                        <input id="Control23" type="text" data-datafield="REFERENCE_NBR" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title15" class="col-md-2">
                        <span id="Label25" data-type="SheetLabel" data-datafield="USER_NAME" style="">账户名</span>
                    </div>
                    <div id="control15" class="col-md-4">
                        <input id="Control25" type="text" data-datafield="USER_NAME" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">制造商</label>
                    </div>
                    <div class="col-md-4" id="asset_zzs">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">车型</label>
                    </div>
                    <div class="col-md-4" id="asset_cx">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">产品类型</label>
                    </div>
                    <div class="col-md-4" id="asset_cplx">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">发动机号码</label>
                    </div>
                    <%--<div class="col-md-4" id="asset_engineNO">
                        <label><span></span></label>
                    </div>--%>
                    <div class="col-md-4">
                        <input type="text" data-datafield="engine_number" data-type="SheetTextBox" id="ctl_engine_number" class="" style=""/>
                    </div>
                </div>
                <div class="row">

                    <div id="title462" class="col-md-2">
                        <span id="Label414" data-type="SheetLabel" data-datafield="branchname" style="">银行分支</span>
                    </div>
                    <div id="control462" class="col-md-4">
                        <select data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-displayemptyitem="true" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">产品组</label>
                    </div>
                    <div class="col-md-4" id="asset_cpz">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">利息总额</label>
                    </div>
                    <div class="col-md-4" id="asset_lxze">
                        <label><span></span></label>
                    </div>
                </div>
                <div class="row bottom">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">应付总额</label>
                    </div>
                    <div class="col-md-4" id="asset_yfze">
                        <label><span></span></label>
                    </div>
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">附加费</label>
                    </div>
                    <div class="col-md-4" id="asset_fjf">
                        <label>
                            <span></span>
                            <a href="#accessoryDetail" data-toggle="modal" onclick="" target="_blank">详细</a>
                        </label>
                    </div>
                </div>
                <div style="display: none;">
                    <input id="Control27" type="text" data-datafield="applicant_type" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                <a href="javascript:void(0);" onclick="hideInfo('sqxx',this)">收起 &and;</a>
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
                    <select data-datafield="Template" data-type="SheetDropDownList" id="ctl371782" class="" style="" data-schemacode="TemplateFile" data-querycode="GetTemplateName" data-filter="WorkflowType:InstanceType" data-datavaluefield="ObjectID" data-datatextfield="TemplateName">
                    </select>
                </div>
                <div id="space10" class="col-md-2">
                    <input type="button" onclick="jumpToEidtDoc('NTKO');" value="打&nbsp印" style="height: 30px; width: 60px" />
                </div>
                <div id="spaceControl10" class="col-md-4">
                </div>
            </div>
            <div class="row tableContent" style="display: none">
                <div id="title11" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="CheckKeyJson" style="">文件名称Json</span>
                </div>
                <div id="Div1" class="col-md-10">
                    <textarea id="Control16" data-datafield="CheckKeyJson" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
            <div class="row">
                <div id="title363" class="col-md-2">
                    <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
                </div>
                <div id="control363" class="col-md-10">
                    <div id="Control343" data-datafield="SFZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title375" class="col-md-2">
                    <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
                </div>
                <div id="control375" class="col-md-10">
                    <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" style="" data-allowbatchdownload="false"></div>
                </div>
            </div>
            <div>
                <div class="row">
                    <div id="title483" class="col-md-2">
                        <span id="Label428" data-type="SheetLabel" data-datafield="MQ" style="">面签照片</span>
                    </div>
                    <div id="control483" class="col-md-10">
                        <div id="Control428" data-datafield="MQ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
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
                        <div id="Control350" data-datafield="HT" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title379" class="col-md-2">
                        <span id="Label351" data-type="SheetLabel" data-datafield="GCFP" style="">购车发票</span>
                    </div>
                    <div id="control379" class="col-md-10">
                        <div id="Control351" data-datafield="GCFP" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title381" class="col-md-2">
                        <span id="Label352" data-type="SheetLabel" data-datafield="BD" style="">保单和保单发票</span>
                    </div>
                    <div id="control381" class="col-md-10">
                        <div id="Control352" data-datafield="BD" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title383" class="col-md-2">
                        <span id="Label353" data-type="SheetLabel" data-datafield="DKHT" style="">贷款合同</span>
                    </div>
                    <div id="control383" class="col-md-10">
                        <div id="Control353" data-datafield="DKHT" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title385" class="col-md-2">
                        <span id="Label354" data-type="SheetLabel" data-datafield="KHDKQEHFSQ" style="">客户贷款全额划付授权书</span>
                    </div>
                    <div id="control385" class="col-md-10">
                        <div id="Control354" data-datafield="KHDKQEHFSQ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title387" class="col-md-2">
                        <span id="Label355" data-type="SheetLabel" data-datafield="KHKKSQS" style="">客户扣款授权书</span>
                    </div>
                    <div id="control387" class="col-md-10">
                        <div id="Control355" data-datafield="KHKKSQS" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title389" class="col-md-2">
                        <span id="Label356" data-type="SheetLabel" data-datafield="GRDKTYTZS" style="">个人贷款同意通知书</span>
                    </div>
                    <div id="control389" class="col-md-10">
                        <div id="Control356" data-datafield="GRDKTYTZS" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>

                <div class="row">
                    <div id="title393" class="col-md-2">
                        <span id="Label358" data-type="SheetLabel" data-datafield="GDHJY" style="">股东会决议（公牌）</span>
                    </div>
                    <div id="control393" class="col-md-10">
                        <div id="Control358" data-datafield="GDHJY" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title473" class="col-md-2">
                        <span id="Label421" data-type="SheetLabel" data-datafield="cldjz" style="">车辆登记证</span>
                    </div>
                    <div id="control473" class="col-md-10">
                        <div id="Control421" data-datafield="cldjz" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title475" class="col-md-2">
                        <span id="Label422" data-type="SheetLabel" data-datafield="yyzz" style="">营业执照</span>
                    </div>
                    <div id="control475" class="col-md-10">
                        <div id="Control422" data-datafield="yyzz" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title477" class="col-md-2">
                        <span id="Label423" data-type="SheetLabel" data-datafield="fjfqrh" style="">附加费确认函</span>
                    </div>
                    <div id="control477" class="col-md-10">
                        <div id="Control423" data-datafield="fjfqrh" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <div class="row">
                    <div id="title391" class="col-md-2">
                        <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">银行卡</span>
                    </div>
                    <div id="control391" class="col-md-10">
                        <div id="Control357" data-datafield="YHKMFYJ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
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
                        <div id="Control437" data-datafield="fjfiqt" data-type="SheetAttachment" style="">
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
                <div class="row">
                    <div id="title469" class="col-md-2">
                        <span id="Label419" data-type="SheetLabel" data-datafield="yyqtfj" style="">运营其他附件</span>
                    </div>
                    <div id="control469" class="col-md-10">
                        <div id="Control419" data-datafield="yyqtfj" data-type="SheetAttachment" style="">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="accessoryDetail" class="modal fade in" aria-hidden="true">
        <div class="modal-header">
            <a class="close" data-dismiss="modal">×</a>
            <h3 style="text-align: center;">附加费详情</h3>
        </div>
        <div class="modal-body">
            <div style="height: auto; min-height: 90px; max-height: 400px; overflow-y: auto;">
        <div class="row tableContent" style="border-right: 0px solid #ccc;border-top: 0px solid #ccc;">
            <div id="control509" class="col-md-10" style="border-left: 0px solid #ccc;width:99%">
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
                                    <input id="Control145_ctl4" type="text" data-datafield="ASSET_ACCESSORY.PRICE" data-type="SheetTextBox" style="width: 92%">
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
            </div>
          
    </div>
    <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
        <label id="Label2" data-en_us="Sheet information">审核信息</label>
    </div>
    <div class="divContent" id="div2">
        <div class="row tableContent">
            <div id="title402" class="col-md-2">
                <span id="Label363" data-type="SheetLabel" data-datafield="ZSYJj" style="">终审意见</span>
            </div>
            <div id="control402" class="col-md-10">
                <div data-datafield="zsshzt" data-type="SheetRadioButtonList" id="ctl2714854" class="" style="" data-defaultitems="核准;拒绝;有条件核准;取消"></div>
                <div id="Control363" data-datafield="ZSYJj" data-type="SheetComment" style="">
                </div>
            </div>
        </div>
        <div class="row tableContent">
            <div id="Div3" class="col-md-2">
                <span id="Span1" data-type="SheetLabel" data-datafield="CSYJ" style="">预审意见</span>
            </div>
            <div id="Div4" class="col-md-10">
                <div data-datafield="yyysshzt" data-type="SheetRadioButtonList" id="Div5" class="" style="" data-defaultitems="核准;拒绝;"></div>

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
            <div id="control401" class="col-md-6">
                <div data-datafield="yyzsshzt" data-type="SheetRadioButtonList" id="ctl271485" class="" style="" data-defaultitems="核准;拒绝;" data-repeatcolumns="3" data-onchange="yyzsChange(this)"></div>
            </div>
            <div id="control501" class="col-md-4">
                <input id="ctDBT" type="checkbox" data-datafield="SFDBT" data-type="SheetCheckbox" class="" style="" data-text="放款承诺函" data-onchange="yyzsChange()">
            </div>
        </div>
    </div>
    <script src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" type="text/javascript"></script>
    <script type="text/javascript">
        function yyzsChange() {
            var ouName = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.OUName"].V;
            if (ouName != "内网经销商") {
                $.MvcSheetUI.GetElement("SFDBT").parent().parent().hide();
                return false
            }
            var state = $.MvcSheetUI.GetControlValue("yyzsshzt");
            var dbt = $.MvcSheetUI.GetControlValue("SFDBT");
            if (state == "拒绝" && dbt) {
                $.MvcSheetUI.SetControlValue("SFDBT", false);
            }
        }
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
            $(".bannerTitle:eq(1)").click();
            hideFIrow();
            getmsg();
            showliuyan();
            $("div[data-isretract='true']").each(function () {
                $(this).find('a').click();
            })
            if ($("#divOriginateOUName").find('label').html() == "外网经销商") {
                $("#divOriginateOUName").find('label').css({ "color": "red" });
            }
            //添加留言
            $('#addmsga').on('click', function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
            setValue();
            //运营终审
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity18") {
                yyzsChange();
            }

            //增加公牌贷的判断
            if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_TYPE_CODE.V == "00002") {
                $("#main-navbar").find("h3").html("机构贷款申请");
            }
            else {
                var isgpd = false;//是否是公牌贷
                $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICANT_TYPE.V.R.forEach(function (v, n) {
                    if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "C") {
                        isgpd = true;
                        return false;
                    }
                });
                if (isgpd) {
                    $("#main-navbar").find("h3").html("个人汽车贷款申请<span style=\"color:red;\">(公牌)</span>");
                }
            }

            setTimeout("lazy_load_jxsed()", 1000);
        }
        function setValue() {
            //新车指导价
            $("#asset_price").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", 1));
            //VIN号
            //$("#asset_vinNo").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.VIN_NUMBER", 1));
            //贷款期数（月）
            $("#asset_term").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1));
            //资产价格
            $("#asset_sale_price").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1));
            //应付余额
            $("#asset_yfye").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.WCYE", 1));
            //首付款比例
            $("#asset_sfkbl").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));

            //首付款比例
            $("#asset_sfkbl").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));
            //首付款金额
            $("#asset_sfkje").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1));
            //贷款额比例%
            $("#asset_dkebl").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", 1));
            //贷款金额
            $("#asset_dkje").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));
            //尾款比例%
            $("#asset_wkbl").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));
            //尾款金额
            $("#asset_wkje").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1));
            //实际利率
            $("#asset_sjll").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACTUAL_RTE", 1));
            //贴息利率
            $("#asset_txll").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CALC_SUBSIDY_RTE", 1));
            //客户利率
            $("#asset_khll").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BASE_CUSTOMER_RATE", 1));
            //贴息金额
            $("#asset_txje").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CALC_SUBSIDY_AMT", 1));

            getApplicantUserInfoByBizObject();
            $("#divBaseInfo").children("div:eq(1)").children("div:visible:last").addClass("bottom");

            //制造商
            $("#asset_zzs").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ASSET_MAKE_DSC", 1));
            //车型
            $("#asset_cx").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ASSET_BRAND_DSC", 1));
            //动力参数
            $("#powerPara").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.POWER_PARAMETER", 1));
            //备注
            $("#modelRemark").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.VEHICLE_COMMENT", 1));

            //发动机号码
            //$("#asset_engineNO").find("span").html($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ENGINE", 1));
            
            //利息总额
            $("#asset_lxze").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ASSETITC", 1));
            //应付总额
            $("#asset_yfze").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.YFJE", 1));
            //附加费
            $("#asset_fjf").find("span").html($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1));

            setTimeout("setDDLValue()", 1000);
        }
        //下拉框取值,有延迟,所以1s后再来执行;
        function setDDLValue() {
            //资产状况
            $("#asset_status").find("span").html($.MvcSheetUI.GetElement("VEHICLE_DETAIL.CONDITION", 1).find("option:selected").text());
            //产品类型
            $("#asset_cplx").find("span").html($.MvcSheetUI.GetElement("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1).find("option:selected").text());
            //产品组
            $("#asset_cpz").find("span").html($.MvcSheetUI.GetElement("CONTRACT_DETAIL.FP_GROUP_ID", 1).find("option:selected").text());
        }

        // 表单验证接口
        $.MvcSheet.Validate = function () {
            var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
            Is_Mortgaged(false);
            getjxsed(false);
            var sfdy = $("#span_sfdy").text();//已经做了抵押的申请不用检查放款额度
            if (this.Action == "Submit" && Activity == 'Activity17' && sfdy == "否") {
                //经销商放款额度检查
                var financedamount = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));    // 根据数据项编码获取页面控件的值
                var kyed = parseFloat($("#span_kyed").text());
                var jieguo = kyed - financedamount;
                if (isNaN(jieguo) || jieguo < 0) {
                    shakeMsg("当前经销商的放款额度不足！");
                    return false;
                }
                return true;
            }
        }
        //申请单是否做抵押
        function Is_Mortgaged(sfyb) {
            var applicationNo = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
            $.ajax({
                //url: "/Portal/ajax/DZBizHandler.ashx",
				url: "/Portal/DZBizHandler/Is_Mortgaged",// 19.6.28 wangxg
                data: { CommandName: "Is_Mortgaged", applicationNo: applicationNo },
                type: "POST",
                async: sfyb,
                dataType: "json",
                success: function (jsonResult) {
                    if (jsonResult.value == 1) {
                        $("#span_sfdy").text("是");
                    }
                    else {
                        $("#span_sfdy").text("否");
                    }
                },
                //error: function (msg) {
                //    shakeMsg(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //获取经销商放款额度
        function getjxsed(sfyb) {
            var nww = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.OUName"].V;
            var jxs = $.MvcSheetUI.GetElement("BUSINESS_PARTNER_ID").find("option:selected").text();
            if (nww.indexOf("内网") > -1) {
                nww = "内网";
            }
            else {
                nww = "外网";
            }
            $.ajax({
                //url: "/Portal/ajax/DZBizHandler.ashx",
				url: "/Portal/DZBizHandler/getjxsed",// 19.6.28 wangxg
                data: { CommandName: "getjxsed", nww: nww, jxs: jxs },
                type: "POST",
                async: sfyb,
                dataType: "json",
                success: function (jsonResult) {
                    //var jsonResult = eval('(' + result + ')');
                    if (jsonResult && jsonResult.length != 0) {
                        $("#span_jxsed").text(jsonResult[0].CSFKED);
                        $("#span_kyed").text(jsonResult[0].KYED);
                        $("#span_ysyed").text(jsonResult[0].YSYED);
                    }
                    else {

                    }
                },
                //error: function (msg) {
                //    shakeMsg(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //延迟加载经销商额度
        function lazy_load_jxsed() {
             //获取经销商开户行
            getjxskfhxx();
            //申请单是否已经做抵押
            Is_Mortgaged(true);
            //经销商放款额度获取
            getjxsed(true);
        }
         //获取经销商开户行，开户行卡号
        function getjxskfhxx() {
            var fi_code = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V;
            $.ajax({
                url: "/Portal/Proposal/getjxskhxx?FI_Code=" + fi_code + "&t=" + new Date().getTime(),
                data: "",
                type: "GET",
                dataType: "json",
                async: true,
                success: function (result) {
                    if (result.jxskhh != "") {
                        $("#span_jxskhh").append( result.jxskhh);
                        $("#span_jxskhhkh").append( result.jxskhhkh);
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
    </script>
</asp:Content>
