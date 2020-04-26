<%@ Page Language="C#" AutoEventWireup="true" CodeFile="YYFKSQ.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.FKSQ" EnableEventValidation="false" MasterPageFile="~/MvcSheetYYGD.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="../../../jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <script src="../../../jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
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

        #accessoryDetail {
            width: 1100px;
            height: 400px;
            background: white;
            overflow: hidden;
            left: 50%;
            top: 50%;
            transform: translate(-50%,-50%);
            border-radius: 3px;
        }

            #accessoryDetail .modal-body {
                width: 100%;
                height: 400px;
                overflow: auto;
                padding: 10px;
            }
    </style>
    <script src="../../../WFRes/_Scripts/TemplatePrint.js"></script>
    <div class="panel-body sheetContainer" style="padding: 0;">
        <div class="nav-icon bannerTitle">
            <label data-en_us="Basic information">申请信息</label>
            <div style="float: right; font-weight: 300; font-size: 14px; padding-right: 5%; cursor: pointer;" onclick="showliuyan()">
                <img src="../../../img/Images/liuyan.png" style="position: relative; top: -4px;" />
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
                <div class="row">
                    <div id="title14" class="col-md-2">
                        <span id="Label24" data-type="SheetLabel" data-datafield="dealerName" style="">经销商</span>
                    </div>
                    <div id="control14" class="col-md-4">
                        <input id="Control24" type="text" data-datafield="dealerName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title1" class="col-md-2">
                        <span id="Label11" data-type="SheetLabel" data-datafield="applicationNo" style="">贷款申请号码</span>
                    </div>
                    <div id="control1" class="col-md-4">
                        <input id="Control11" type="text" data-datafield="applicationNo" data-type="SheetTextBox" style="">
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
                    <div id="title304" class="col-md-2">
                        <span id="Label288" data-type="SheetLabel" data-datafield="assetConditionName" style="">资产状况</span>
                    </div>
                    <div id="control304" class="col-md-4">
                        <input id="Control288" type="text" data-datafield="assetConditionName" data-type="SheetTextBox" style="">
                    </div>
                </div>

                <div class="row">
                    <div id="title309" class="col-md-2">
                        <span id="Label293" data-type="SheetLabel" data-datafield="assetPrice" style="">新车指导价</span>
                    </div>
                    <div id="control309" class="col-md-4">
                        <input id="Control293" type="text" data-datafield="assetPrice" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title314" class="col-md-2">
                        <span id="Label298" data-type="SheetLabel" data-datafield="vinNo" style="">VIN号</span>
                    </div>
                    <div id="control314" class="col-md-4">
                        <input id="Control298" type="text" onchange="yzlth(this)" data-datafield="vinNo" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">

                    <div id="title339" class="col-md-2">
                        <span id="Label321" data-type="SheetLabel" data-datafield="termMonth" style="">贷款期数（月）</span>
                    </div>
                    <div id="control339" class="col-md-4">
                        <input id="Control321" type="text" data-datafield="termMonth" data-type="SheetTextBox" style="">
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
                    <div id="title342" class="col-md-2">
                        <span id="Label324" data-type="SheetLabel" data-datafield="salesprice" style="">资产价格</span>
                    </div>
                    <div id="control342" class="col-md-4">
                        <input id="Control324" type="text" data-datafield="salesprice" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title358" class="col-md-2">
                        <span id="Label340" data-type="SheetLabel" data-datafield="BalancePayable" style="">应付余额</span>
                    </div>
                    <div id="control358" class="col-md-4">
                        <input id="Control340" type="text" data-datafield="BalancePayable" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title344" class="col-md-2">
                        <span id="Label326" data-type="SheetLabel" data-datafield="downpaymentrate" style="">首付款比例 %</span>
                    </div>
                    <div id="control344" class="col-md-4">
                        <input id="Control326" type="text" data-datafield="downpaymentrate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title346" class="col-md-2">
                        <span id="Label328" data-type="SheetLabel" data-datafield="downpaymentamount" style="">首付款金额</span>
                    </div>
                    <div id="control346" class="col-md-4">
                        <input id="Control328" type="text" data-datafield="downpaymentamount" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title350" class="col-md-2">
                        <span id="Label332" data-type="SheetLabel" data-datafield="financedamountrate" style="">贷款额比例%</span>
                    </div>
                    <div id="control350" class="col-md-4">
                        <input id="Control332" type="text" data-datafield="financedamountrate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title348" class="col-md-2">
                        <span id="Label330" data-type="SheetLabel" data-datafield="financedamount" style="">贷款金额</span>
                    </div>
                    <div id="control348" class="col-md-4">
                        <input id="Control330" type="text" data-datafield="financedamount" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title343" class="col-md-2">
                        <span id="Label325" data-type="SheetLabel" data-datafield="balloonRate" style="">尾款比例%</span>
                    </div>
                    <div id="control343" class="col-md-4">
                        <input id="Control325" type="text" data-datafield="balloonRate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title345" class="col-md-2">
                        <span id="Label327" data-type="SheetLabel" data-datafield="balloonAmount" style="">尾款金额</span>
                    </div>
                    <div id="control345" class="col-md-4">
                        <input id="Control327" type="text" data-datafield="balloonAmount" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title352" class="col-md-2">
                        <span id="Label334" data-type="SheetLabel" data-datafield="actualRate" style="">实际利率</span>
                    </div>
                    <div id="control352" class="col-md-4">
                        <input id="Control334" type="text" data-datafield="actualRate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title354" class="col-md-2">
                        <span id="Label336" data-type="SheetLabel" data-datafield="subsidyRate" style="">贴息利率</span>
                    </div>
                    <div id="control354" class="col-md-4">
                        <input id="Control336" type="text" data-datafield="subsidyRate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title347" class="col-md-2">
                        <span id="Label329" data-type="SheetLabel" data-datafield="interestRate" style="">客户利率</span>
                    </div>
                    <div id="control347" class="col-md-4">
                        <input id="Control329" type="text" data-datafield="interestRate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title349" class="col-md-2">
                        <span id="Label331" data-type="SheetLabel" data-datafield="subsidyAmount" style="">贴息金额</span>
                    </div>
                    <div id="control349" class="col-md-4">
                        <input id="Control331" type="text" data-datafield="subsidyAmount" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title21" class="col-md-2">
                        <span id="Label29" data-type="SheetLabel" data-datafield="ThaiFirstName" style="">借款人姓名</span>
                    </div>
                    <div id="control21" class="col-md-4">
                        <input id="Control29" type="text" data-datafield="ThaiFirstName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title23" class="col-md-2">
                        <span id="Label31" data-type="SheetLabel" data-datafield="IdCardNo" style="">证件号码</span>
                    </div>
                    <div id="control23" class="col-md-4">
                        <input id="Control31" type="text" data-datafield="IdCardNo" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title123" class="col-md-2">
                        <span id="Label122" data-type="SheetLabel" data-datafield="GjThaiFirstName" style="">共借人姓名</span>
                    </div>
                    <div id="control123" class="col-md-4">
                        <input id="Control122" type="text" data-datafield="GjThaiFirstName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title125" class="col-md-2">
                        <span id="Label124" data-type="SheetLabel" data-datafield="GjIdCardNo" style="">共借人证件号码</span>
                    </div>
                    <div id="control125" class="col-md-4">
                        <input id="Control124" type="text" data-datafield="GjIdCardNo" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title217" class="col-md-2">
                        <span id="Label209" data-type="SheetLabel" data-datafield="DbThaiFirstName" style="">担保人姓名</span>
                    </div>
                    <div id="control217" class="col-md-4">
                        <input id="Control209" type="text" data-datafield="DbThaiFirstName" data-type="SheetTextBox" style="">
                    </div>

                    <div id="title219" class="col-md-2">
                        <span id="Label211" data-type="SheetLabel" data-datafield="DbIdCardNo" style="">担保人证件号码</span>
                    </div>
                    <div id="control219" class="col-md-4">
                        <input id="Control211" type="text" data-datafield="DbIdCardNo" data-type="SheetTextBox" style="">
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
                        <span id="Label12" data-type="SheetLabel" data-datafield="appTypeName" style="">申请类型</span>
                    </div>
                    <div id="control2" class="col-md-4">
                        <input id="Control12" type="text" data-datafield="appTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title4" class="col-md-2">
                        <span id="Label14" data-type="SheetLabel" data-datafield="companyName" style="">申请贷款公司</span>
                    </div>
                    <div id="control4" class="col-md-4">
                        <input id="Control14" type="text" data-datafield="companyName" data-type="SheetTextBox" style="">
                    </div>

                    <div id="title13" class="col-md-2">
                        <span id="Label23" data-type="SheetLabel" data-datafield="refCANumber" style="">申请参考号</span>
                    </div>
                    <div id="control13" class="col-md-4">
                        <input id="Control23" type="text" data-datafield="refCANumber" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title15" class="col-md-2">
                        <span id="Label25" data-type="SheetLabel" data-datafield="userName" style="">账户名</span>
                    </div>
                    <div id="control15" class="col-md-4">
                        <input id="Control25" type="text" data-datafield="userName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title305" class="col-md-2">
                        <span id="Label289" data-type="SheetLabel" data-datafield="assetMakeName" style="">制造商</span>
                    </div>
                    <div id="control305" class="col-md-4">
                        <input id="Control289" type="text" data-datafield="assetMakeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title306" class="col-md-2">
                        <span id="Label290" data-type="SheetLabel" data-datafield="brandName" style="">车型</span>
                    </div>
                    <div id="control306" class="col-md-4">
                        <input id="Control290" type="text" data-datafield="brandName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title336" class="col-md-2">
                        <span id="Label318" data-type="SheetLabel" data-datafield="productTypeName" style="">产品类型</span>
                    </div>
                    <div id="Div6" class="col-md-4">
                        <input id="Control318" type="text" data-datafield="productTypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title313" class="col-md-2">
                        <span id="Label297" data-type="SheetLabel" data-datafield="engineNo" style="">发动机号码</span>
                    </div>
                    <div id="control313" class="col-md-4">
                        <input id="Control297" type="text" data-datafield="engineNo" data-type="SheetTextBox" style="">
                    </div>
                    <%--<div id="title314" class="col-md-2">
                    <span id="Label298" data-type="SheetLabel" data-datafield="vinNo" style="">VIN号</span>
                </div>
                <div id="control314" class="col-md-4">
                    <input id="Control298" type="text" onchange="yzlth(this)" data-datafield="vinNo" data-type="SheetTextBox" style="">
                </div>--%>
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
                    <div id="title335" class="col-md-2">
                        <span id="Label317" data-type="SheetLabel" data-datafield="productGroupName" style="">产品组</span>
                    </div>
                    <div id="control335" class="col-md-4">
                        <input id="Control317" type="text" data-datafield="productGroupName" data-type="SheetTextBox" style="">
                    </div>
                    <%--<div id="title336" class="col-md-2">
                    <span id="Label318" data-type="SheetLabel" data-datafield="productTypeName" style="">产品类型</span>
                </div>
                <div id="control336" class="col-md-4">
                    <input id="Control318" type="text" data-datafield="productTypeName" data-type="SheetTextBox" style="">
                </div>--%>
                </div>
                <div class="row">
                    <div id="title353" class="col-md-2">
                        <span id="Label335" data-type="SheetLabel" data-datafield="totalintamount" style="">利息总额</span>
                    </div>
                    <div id="control353" class="col-md-4">
                        <input id="Control335" type="text" data-datafield="totalintamount" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title355" class="col-md-2">
                        <span id="Label337" data-type="SheetLabel" data-datafield="totalamountpayable" style="">不存在</span>
                    </div>
                    <div id="control355" class="col-md-4">
                        <input id="Control337" type="text" data-datafield="totalamountpayable" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title330" class="col-md-2">
                        <span id="Label314" data-type="SheetLabel" data-datafield="totalaccessoryamount" style="">附加费</span>
                    </div>
                    <div id="control330" class="col-md-4">
                        <input id="Control314" type="text" data-datafield="totalaccessoryamount" data-type="SheetTextBox" style="">
                        <a href="#accessoryDetail" data-toggle="modal" onclick="" target="_blank">详细</a>
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

                <%--                <div class="row">
                    <div id="title361" class="col-md-2">
                        <span id="Label342" data-type="SheetLabel" data-datafield="SQD" style="">申请单资料</span>
                    </div>
                    <div id="control361" class="col-md-10">
                        <div id="Control342" data-datafield="SQD" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title365" class="col-md-2">
                        <span id="Label344" data-type="SheetLabel" data-datafield="JHZ" style="">婚姻类材料</span>
                    </div>
                    <div id="control365" class="col-md-10">
                        <div id="Control344" data-datafield="JHZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title367" class="col-md-2">
                        <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
                    </div>
                    <div id="control367" class="col-md-10">
                        <div id="Control345" data-datafield="JSZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title369" class="col-md-2">
                        <span id="Label346" data-type="SheetLabel" data-datafield="JZZ" style="">居住类材料</span>
                    </div>
                    <div id="control369" class="col-md-10">
                        <div id="Control346" data-datafield="JZZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title371" class="col-md-2">
                        <span id="Label347" data-type="SheetLabel" data-datafield="SR" style="">收入类材料</span>
                    </div>
                    <div id="control371" class="col-md-10">
                        <div id="Control347" data-datafield="SR" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title373" class="col-md-2">
                        <span id="Label348" data-type="SheetLabel" data-datafield="GZ" style="">工作证明\企业类证明</span>
                    </div>
                    <div id="control373" class="col-md-10">
                        <div id="Control348" data-datafield="GZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title375" class="col-md-2">
                        <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
                    </div>
                    <div id="control375" class="col-md-10">
                        <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>--%>
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
    <div id="accessoryDetail" class="modal fade in" aria-hidden="true">
        <div class="modal-header">
            <a class="close" data-dismiss="modal">×</a>
            <h3 style="text-align: center;">附加费详情</h3>
        </div>
        <div class="modal-body">
            <div style="height: auto; min-height: 90px; max-height: 400px; overflow-y: auto;">
                <div class="row tableContent" style="border-right: 0px solid #ccc; border-top: 0px solid #ccc;">
                    <div id="control509" class="col-md-10" style="border-left: 0px solid #ccc; width: 99%">
                        <table id="Control444" data-datafield="RentalAssetAccessoriesTable" data-type="SheetGridView" style="width: 99%;" class="SheetGridView">
                            <tbody>
                                <tr class="header">
                                    <td id="Control444_SerialNo" class="rowSerialNo">序号								</td>
                                    <td id="Control444_Header4" data-datafield="RentalAssetAccessoriesTable.additionalName1">
                                        <label id="Control444_Label4" data-datafield="RentalAssetAccessoriesTable.additionalName1" data-type="SheetLabel" style="">选装部件</label>
                                    </td>
                                    <td id="Control444_Header5" data-datafield="RentalAssetAccessoriesTable.additionalprice">
                                        <label id="Control444_Label5" data-datafield="RentalAssetAccessoriesTable.additionalprice" data-type="SheetLabel" style="">价格</label>
                                    </td>
                                    <td class="rowOption">删除								</td>
                                </tr>
                                <tr class="template">
                                    <td id="Control444_Option" class="rowOption"></td>
                                    <td data-datafield="RentalAssetAccessoriesTable.additionalName1">
                                        <input id="Control444_ctl4" type="text" data-datafield="RentalAssetAccessoriesTable.additionalName1" data-type="SheetTextBox" style="">
                                    </td>
                                    <td data-datafield="RentalAssetAccessoriesTable.additionalprice">
                                        <input id="Control444_ctl5" type="text" data-datafield="RentalAssetAccessoriesTable.additionalprice" data-type="SheetTextBox" style="">
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
                                <tr class="footer">
                                    <td class="rowOption"></td>
                                    <td data-datafield="RentalAssetAccessoriesTable.additionalName1"></td>
                                    <td data-datafield="RentalAssetAccessoriesTable.additionalprice">
                                        <label id="Control444_stat5" data-datafield="RentalAssetAccessoriesTable.additionalprice" data-type="SheetCountLabel" style="">
                                        </label>
                                    </td>
                                    <td class="rowOption"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

    </div>
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
        function showliuyan() {
            if ($("#showliuyan").is(":visible")) {
                $("#showliuyan").hide();
                $("#lysq").hide();
            } else {
                $("#showliuyan").show();
                $("#lysq").show();
            }
        }
        function hideInfo(id, ctrl1) {
            if ($(ctrl1).html() == '展开更多 ∨') {
                $(ctrl1).html("收起 &and;");
                $("#" + id).show();
            }
            else {
                $(ctrl1).html("展开更多 &or;");
                $("#" + id).hide();
            }
        }
        function hidediv(id, ctrl1) {
            var ctrl = document.getElementById(id);
            if ($(ctrl).css("display") != 'none') {
                $(ctrl).hide();
                //class ="nav-icon fa fa-chevron-down bannerTitle" 
                if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
                    $(ctrl1).removeClass("fa-angle-double-down");
                    $(ctrl1).addClass("fa-angle-double-right");
                }
                else {
                    $(ctrl1).removeClass("fa-chevron-down");
                    $(ctrl1).addClass("fa-chevron-right");
                }
            }
            else {
                $(ctrl).show();
                if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
                    $(ctrl1).removeClass("fa-angle-double-right");
                    $(ctrl1).addClass("fa-angle-double-down");
                }
                else {
                    $(ctrl1).removeClass("fa-chevron-right");
                    $(ctrl1).addClass("fa-chevron-down");
                }
            }
        }
        //获取留言信息
        function getmsg() {
            var InstanceId = '<%=this.ActionContext.InstanceId%>';
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getLYInfo",// 19.6.28 wangxg
                data: { CommandName: "getLYInfo", instanceid: InstanceId },
                type: "POST",
                async: false,
                dataType: "text",
                success: function (result) {
                    result = JSON.parse(result.replace(/\n/g, ""));
                    $("#liuyan").find("tr:eq(0)").nextAll().remove();
                    for (var i = 0; i < result.length; i++) {
                        var j = i + 1;
                        var tr = "<tr>";
                        tr += "<td>" + j + "</td>";
                        tr += "<td><label>" + result[i].LYXX + "</label></td>";
                        tr += "<td><label>" + result[i].USERNAME + "</label></td>";
                        tr += "<td><label>" + result[i].LYSJ.replace(/\//g, "\-") + "</label></td>";
                        tr += "</tr>";
                        $("#liuyan").append(tr);
                    }
                },
                //error: function (msg) {
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //添加留言
        function addmsg() {
            var userid = '<%=this.ActionContext.User.UserID%>';
            var InstanceId = '<%=this.ActionContext.InstanceId%>';
            var msgval = $("#addmsg").val();
            if (!$("#addmsg").val() || $("#addmsg").val() == "") {
                alert("请先填写信息！");
                return false;
            }
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/insertLYInfo",// 19.6.28 wangxg
                data: { CommandName: "insertLYInfo", userid: userid, instanceid: InstanceId, ly: msgval },
                type: "POST",
                async: false,
                dataType: "json",
                success: function (result) {
                    $("#addmsg").val("");
                    getmsg();
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //VIN验证17位长度
        function yzlth(ts) {
            debugger;
            if ($(ts).val().length != 17) {
                if ($(ts).val() == "") {
                    return;
                }
                $(ts).val("");
                alert("VIN号长度不是17位!");
            }
        }
        //附件详情
        function getDownLoadURL() {
            if ($("#divattachment").find("tr").length == 0) {
                alert("附件为空！");
            } else {
                window.open("../view/FI.html");
            }
            event.stopPropagation();
        }
        //银行卡数字验证
        function textnum(ts) {
            if (!/^[0-9]*$/.test($(ts).val())) {
                $(ts).val("");
                alert("请输入数字!");
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
            //隐藏批量下载
            if ($.MvcSheetUI.SheetInfo.ActivityCode != "FI1") {
                $(".SheetAttachment").find("a").hide();
            }
            var Activity = '<%=this.ActionContext.ActivityCode%>';
            var IsWork = $.MvcSheetUI.QueryString("Mode");
            if (Activity == 'Activity48' && IsWork == "work") {
                var m1 = $("#Control29").val();
                var opt1 = "<option value=\"" + m1 + "\">" + m1 + "</option>";
                var m2 = $("#Control122").val();
                var opt2 = "<option value=\"" + m2 + "\">" + m2 + "</option>";
                $("#Control415s").append(opt1);
                $("#Control415s").append(opt2);
                $("#Control415s").show();
                $("#Control415s").val("");
                $("#Control415s").parent().find(">input").hide();
                if ($("#Control415").val() && $("#Control415").val().length > 0) {
                    $("#Control415s").val($("#Control415").val());
                }
            }
            $(".bannerTitle:eq(1)").click();

            if (Activity == 'Activity48') {
                if (IsMobile) {
                    $("#ctl64724").parent().addClass("chkbank");
                    $("#ctl875733").parent().addClass("chkbank");
                }
                $("#divattachment").hide();
                $("#divattachmentinfo").hide();
            }
            hideFIrow();
            getmsg();
            showliuyan();
            $("div[data-isretract='true']").each(function () {
                $(this).find('a').click();
            })
            //共借人为机构贷
            if ($("#Control27").val() && $("#Control27").val() != "") {
                $("#main-navbar").find("h3").html("个人汽车贷款申请<span style=\"color:red;\">(公牌)</span>");
            }
            if ($("#divOriginateOUName").find('label').html() == "外网经销商") {
                $("#divOriginateOUName").find('label').css({ "color": "red" });
            }
            //添加留言
            $('#addmsga').on('click', function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
            //申请单是否已经做抵押
            Is_Mortgaged(true);
            //经销商放款额度获取
            getjxsed(true);
        }
        // 保存事件
        $.MvcSheet.SaveAction.Save = function () {
            // this.Action  // 获取当前按钮名称
            alert($.MvcSheetUI.SheetInfo.InstanceId);
            $.MvcSheetUI.SetControlValue("CJH", $.MvcSheetUI.SheetInfo.InstanceId);
        }
        // 表单验证接口
        $.MvcSheet.Validate = function () {
            var Activity = '<%=this.ActionContext.ActivityCode%>';
            // this.Action 表示当前操作的按钮名称
            Is_Mortgaged(false);
            getjxsed(false);
            var sfdy = $("#span_sfdy").text();//已经做了抵押的申请不用检查放款额度
            if (this.Action == "Submit" && Activity == 'Activity17' && sfdy == "否") {
                //经销商放款额度检查
                var financedamount = parseFloat($.MvcSheetUI.GetControlValue("financedamount"));    // 根据数据项编码获取页面控件的值
                var kyed = parseFloat($("#span_kyed").text());
                var jieguo = kyed - financedamount;
		if (isNaN(jieguo) || jieguo < 0) {
                    alert("当前经销商的放款额度不足！");
                    return false;
                }
                return true;
            }
        }
        $.MvcSheet.SaveAction.OnActionDone = function () {
            // this.Action  // 获取当前按钮名称
            //alert($.MvcSheetUI.SheetInfo.InstanceId);
            //$.MvcSheetUI.SetControlValue("instanceid", $.MvcSheetUI.SheetInfo.InstanceId);
        }
        //申请单是否做抵押
        function Is_Mortgaged(sfyb) {
            var applicationNo = $.MvcSheetUI.GetControlValue("applicationNo");
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/Is_Mortgaged",// 19.6.28 wangxg
                data: { CommandName: "Is_Mortgaged", applicationNo: applicationNo },
                type: "POST",
                async: sfyb,
                dataType: "json",
                success: function (jsonResult) {
                    //var jsonResult = eval('(' + result + ')');
                    if (jsonResult.value == 1) {
                        $("#span_sfdy").text("是");
                    }
                    else {
                        $("#span_sfdy").text("否");
                    }
                },
                //error: function (msg) {
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //获取经销商放款额度
        function getjxsed(sfyb) {
            var nww = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.OUName"].V;
            if (nww.indexOf("内网") > -1) {
                nww = "内网";
            }
            else {
                nww = "外网";
            }
            var jxs = $.MvcSheetUI.GetControlValue("dealerName");
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getjxsed",// 19.6.28 wangxg
                data: { CommandName: "getjxsed", nww: nww, jxs: jxs },
                type: "POST",
                async: sfyb,
                dataType: "json",
                success: function (jsonResult) {
                    //var jsonResult = eval('(' + result + ')');
                    if (jsonResult && jsonResult.length !=0) {
                        $("#span_jxsed").text(jsonResult[0].CSFKED);
                        $("#span_kyed").text(jsonResult[0].KYED);
                        $("#span_ysyed").text(jsonResult[0].YSYED);
                    }
                    else {
                        $("#span_jxsed").text("0");
                        $("#span_kyed").text("0");
                    }
                },
                //error: function (msg) {
                //    $("#span_jxsed").text("0");
                //    $("#span_kyed").text("0");
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
                    $("#span_jxsed").text("0");
                    $("#span_kyed").text("0");
					 showJqErr(msg);
                }
            });
        }
    </script>
</asp:Content>
