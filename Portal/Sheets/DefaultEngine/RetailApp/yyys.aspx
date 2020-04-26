<%@ Page Language="C#" AutoEventWireup="true" CodeFile="yyys.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.yyys" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

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
            display: inline-block!important;
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
            display: block!important;
        }

        .chkbank span {
            display: none!important;
        }
    </style>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">个人汽车贷款申请</label>
    </div>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divBaseInfo',this)">
            <label data-en_us="Basic information">基本信息</label>
        </div>
        <div class="divContent" id="divBaseInfo">
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
            <div class="row">
                <div id="title1" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="applicationNo" style="">贷款申请号码</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input id="Control11" type="text" data-datafield="applicationNo" data-type="SheetTextBox" style="">
                </div>
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
                <div id="title14" class="col-md-2">
                    <span id="Label24" data-type="SheetLabel" data-datafield="dealerName" style="">经销商</span>
                </div>
                <div id="Div1" class="col-md-4">
                    <input id="Control24" type="text" data-datafield="dealerName" data-type="SheetTextBox" style="">
                </div>

                <div id="title15" class="col-md-2">
                    <span id="Label25" data-type="SheetLabel" data-datafield="userName" style="">账户名</span>
                </div>
                <div id="control15" class="col-md-4">
                    <input id="Control25" type="text" data-datafield="userName" data-type="SheetTextBox" style="">
                </div>

            </div>
            <div class="row">
                <div id="title21" class="col-md-2">
                    <span id="Label29" data-type="SheetLabel" data-datafield="ThaiFirstName" style="">姓名（中文）</span>
                </div>
                <div id="control21" class="col-md-4">
                    <input id="Control29" type="text" data-datafield="ThaiFirstName" data-type="SheetTextBox" style="">
                </div>

                <div id="title23" class="col-md-2">
                    <span id="Label31" data-type="SheetLabel" data-datafield="IdCardNo" style="">证件号码</span>
                </div>
                <div id="Div2" class="col-md-4">
                    <input id="Control31" type="text" data-datafield="IdCardNo" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title123" class="col-md-2">
                    <span id="Label122" data-type="SheetLabel" data-datafield="GjThaiFirstName" style="">共借人姓名（中文）</span>
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
            
			<div class="row">
				<div id="title421" class="col-md-2">
					<span id="Label379" data-type="SheetLabel" data-datafield="fkResult" style="">风控审核结果</span>
				</div>
				<div id="control421" class="col-md-4">
					<input id="Control379" type="text" data-datafield="fkResult" data-type="SheetTextBox" style="">
				</div>
				<div id="Div5" class="col-md-2">
					<span id="Span3" data-type="SheetLabel" data-datafield="phoneNo" style="">申请人电话</span>
				</div>
				<div id="Div6" class="col-md-4">
					<input id="Text5" type="text" data-datafield="phoneNo" data-type="SheetTextBox" style="">
				</div>
			</div>
           
            <div class="row">
                <div id="title514" class="col-md-2">
                    <span id="Label448" data-type="SheetLabel" data-datafield="dysf" style="">抵押省份</span>
                </div>
                <div id="control514" class="col-md-4">
                    <select data-datafield="dysf" data-type="SheetDropDownList" id="ctl699489" class="" style="" data-displayemptyitem="true" data-schemacode="area" data-querycode="area" data-filter="100000:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME">
                    </select>
                </div>
                <div id="title515" class="col-md-2">
                    <span id="Label449" data-type="SheetLabel" data-datafield="dycs" style="">抵押城市</span>
                </div>
                <div id="control515" class="col-md-4">
                    <select data-datafield="dycs" data-type="SheetDropDownList" id="ctl53508" class="" style="" data-displayemptyitem="true" data-schemacode="area" data-querycode="area" data-filter="dysf:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="title516" class="col-md-2">
                    <span id="Label450" data-type="SheetLabel" data-datafield="dyx" style="">抵押县</span>
                </div>
                <div id="control516" class="col-md-4">
                    <select data-datafield="dyx" data-type="SheetDropDownList" id="ctl291016" class="" style="" data-displayemptyitem="true" data-schemacode="area" data-querycode="area" data-filter="dycs:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME">
                    </select>
                </div>
                <div id="Div8" class="col-md-2">
                </div>
                <div id="Div9" class="col-md-4">
                </div>
            </div>
            <div class="row">
                <div id="title445" class="col-md-12">
                    <span id="Label398" data-type="SheetLabel" data-datafield="LY" class="">留言信息</span>
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
                        <br />
                        <a href="javascript:void(0);" id="addmsga">提交</a>
                    </div>
                </div>
            </div>
            <div class="row tableContent">
                    <div id="title425" class="leftbox1">
                        <span id="Label382" data-type="SheetLabel" data-datafield="THJL" style="">电调记录</span>
                    </div>
                    <div id="control425" class="rightbox1">
                        <textarea id="Control382" data-datafield="THJL" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
        </div>
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divZC',this)" style="display:none">
            <label id="Label3" data-en_us="Basic information">资产信息</label>
        </div>
        <div class="divContent" id="divZC" style="display:none">
            <div class="row">
                <div id="title304" class="col-md-2">
                    <span id="Label288" data-type="SheetLabel" data-datafield="assetConditionName" style="">资产状况</span>
                </div>
                <div id="control304" class="col-md-4">
                    <input id="Control288" type="text" data-datafield="assetConditionName" data-type="SheetTextBox" style="">
                </div>
                <div id="title305" class="col-md-2">
                    <span id="Label289" data-type="SheetLabel" data-datafield="assetMakeName" style="">制造商</span>
                </div>
                <div id="control305" class="col-md-4">
                    <input id="Control289" type="text" data-datafield="assetMakeName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title306" class="col-md-2">
                    <span id="Label290" data-type="SheetLabel" data-datafield="brandName" style="">车型</span>
                </div>
                <div id="control306" class="col-md-4">
                    <input id="Control290" type="text" data-datafield="brandName" data-type="SheetTextBox" style="">
                </div>
                <div id="title309" class="col-md-2">
                    <span id="Label293" data-type="SheetLabel" data-datafield="assetPrice" style="">资产价格</span>
                </div>
                <div id="control309" class="col-md-4">
                    <input id="Control293" type="text" data-datafield="assetPrice" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title313" class="col-md-2">
                    <span id="Label297" data-type="SheetLabel" data-datafield="engineNo" style="">发动机号码</span>
                </div>
                <div id="control313" class="col-md-4">
                    <input id="Control297" type="text" data-datafield="engineNo" data-type="SheetTextBox" style="">
                </div>
                <div id="title314" class="col-md-2">
                    <span id="Label298" data-type="SheetLabel" data-datafield="vinNo" style="">VIN号</span>
                </div>
                <div id="control314" class="col-md-4">
                    <input id="Control298" type="text" onchange="yzlth(this)" data-datafield="vinNo" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title461" class="col-md-2">
                    <span id="Label413" data-type="SheetLabel" data-datafield="Bankname" style="">扣款银行</span>
                </div>
                <div id="control461" class="col-md-4">
                    <select data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-displayemptyitem="true" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>
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
                </div>
                <div id="title464" class="col-md-2">
                    <span id="Label416" data-type="SheetLabel" data-datafield="AccoutNumber" style="">账户号</span>
                </div>
                <div id="control464" class="col-md-4">
                    <input id="Control416" type="text" oninput="textnum(this)" data-datafield="AccoutNumber" data-type="SheetTextBox" style="">
                </div>
            </div>
        </div>
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divJRTK',this)" style="display:none">
            <label id="Label4" data-en_us="Basic information">金融条款</label>
        </div>
        <div class="divContent" id="divJRTK" style="display:none">
            <div class="row">
                <div id="title335" class="col-md-2">
                    <span id="Label317" data-type="SheetLabel" data-datafield="productGroupName" style="">产品组</span>
                </div>
                <div id="control335" class="col-md-4">
                    <input id="Control317" type="text" data-datafield="productGroupName" data-type="SheetTextBox" style="">
                </div>
                <div id="title336" class="col-md-2">
                    <span id="Label318" data-type="SheetLabel" data-datafield="productTypeName" style="">产品类型</span>
                </div>
                <div id="control336" class="col-md-4">
                    <input id="Control318" type="text" data-datafield="productTypeName" data-type="SheetTextBox" style="">
                </div>
            </div>

            <div class="row">
                <div id="title339" class="col-md-2">
                    <span id="Label321" data-type="SheetLabel" data-datafield="termMonth" style="">贷款期数（月）</span>
                </div>
                <div id="control339" class="col-md-4">
                    <input id="Control321" type="text" data-datafield="termMonth" data-type="SheetTextBox" style="">
                </div>
                <div id="title342" class="col-md-2">
                    <span id="Label324" data-type="SheetLabel" data-datafield="vehicleprice" style="">销售价格</span>
                </div>
                <div id="control342" class="col-md-4">
                    <input id="Control324" type="text" data-datafield="vehicleprice" data-type="SheetTextBox" style="">
                </div>
            </div>

            <div class="row">
                <div id="title343" class="col-md-2">
                    <span id="Label325" data-type="SheetLabel" data-datafield="balloonRate" style="">资产残值/轻松融资尾款%</span>
                </div>
                <div id="control343" class="col-md-4">
                    <input id="Control325" type="text" data-datafield="balloonRate" data-type="SheetTextBox" style="">
                </div>
                <div id="title344" class="col-md-2">
                    <span id="Label326" data-type="SheetLabel" data-datafield="downpaymentrate" style="">首付款比例 %</span>
                </div>
                <div id="control344" class="col-md-4">
                    <input id="Control326" type="text" data-datafield="downpaymentrate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title345" class="col-md-2">
                    <span id="Label327" data-type="SheetLabel" data-datafield="balloonAmount" style="">资产残值/轻松融资尾款金额</span>
                </div>
                <div id="control345" class="col-md-4">
                    <input id="Control327" type="text" data-datafield="balloonAmount" data-type="SheetTextBox" style="">
                </div>
                <div id="title346" class="col-md-2">
                    <span id="Label328" data-type="SheetLabel" data-datafield="downpaymentamount" style="">首付款金额</span>
                </div>
                <div id="control346" class="col-md-4">
                    <input id="Control328" type="text" data-datafield="downpaymentamount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title347" class="col-md-2">
                    <span id="Label329" data-type="SheetLabel" data-datafield="interestRate" style="">客户利率% </span>
                </div>
                <div id="control347" class="col-md-4">
                    <input id="Control329" type="text" data-datafield="interestRate" data-type="SheetTextBox" style="">
                </div>
                <div id="title348" class="col-md-2">
                    <span id="Label330" data-type="SheetLabel" data-datafield="financedamount" style="">贷款金额</span>
                </div>
                <div id="control348" class="col-md-4">
                    <input id="Control330" type="text" data-datafield="financedamount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title349" class="col-md-2">
                    <span id="Label331" data-type="SheetLabel" data-datafield="subsidyAmount" style="">贴息金额</span>
                </div>
                <div id="control349" class="col-md-4">
                    <input id="Control331" type="text" data-datafield="subsidyAmount" data-type="SheetTextBox" style="">
                </div>
                <div id="title350" class="col-md-2">
                    <span id="Label332" data-type="SheetLabel" data-datafield="financedamountrate" style="">贷款额比例%</span>
                </div>
                <div id="control350" class="col-md-4">
                    <input id="Control332" type="text" data-datafield="financedamountrate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title358" class="col-md-2">
                    <span id="Label340" data-type="SheetLabel" data-datafield="BalancePayable" style="">未偿余额</span>
                </div>
                <div id="control358" class="col-md-4">
                    <input id="Control340" type="text" data-datafield="BalancePayable" data-type="SheetTextBox" style="">
                </div>
                <div id="title352" class="col-md-2">
                    <span id="Label334" data-type="SheetLabel" data-datafield="actualRate" style="">实际利率</span>
                </div>
                <div id="control352" class="col-md-4">
                    <input id="Control334" type="text" data-datafield="actualRate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title353" class="col-md-2">
                    <span id="Label335" data-type="SheetLabel" data-datafield="totalintamount" style="">利息总额</span>
                </div>
                <div id="control353" class="col-md-4">
                    <input id="Text1" type="text" data-datafield="totalintamount" data-type="SheetTextBox" style="">
                </div>
                <div id="title354" class="col-md-2">
                    <span id="Label336" data-type="SheetLabel" data-datafield="subsidyRate" style="">贴息利率</span>
                </div>
                <div id="control354" class="col-md-4">
                    <input id="Text2" type="text" data-datafield="subsidyRate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title355" class="col-md-2">
                    <span id="Label337" data-type="SheetLabel" data-datafield="totalamountpayable" style="">应付总额</span>
                </div>
                <div id="control355" class="col-md-4">
                    <input id="Control337" type="text" data-datafield="totalamountpayable" data-type="SheetTextBox" style="">
                </div>
                <div id="title330" class="col-md-2">
                    <span id="Label314" data-type="SheetLabel" data-datafield="totalaccessoryamount" style="">附加费</span>
                </div>
                <div id="Div3" class="col-md-4">
                    <input id="Text3" type="text" data-datafield="totalaccessoryamount" data-type="SheetTextBox" style="">
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
            <div>
                 <div class="row">
                    <div id="title383" class="col-md-2">
                        <span id="Label353" data-type="SheetLabel" data-datafield="DKHT" style="">贷款合同</span>
                    </div>
                    <div id="control383" class="col-md-10">
                        <div id="Div7" data-datafield="DKHT" data-type="SheetAttachment" style=""   data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                 <div class="row">
                <div id="title363" class="col-md-2">
                    <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
                </div>
                <div id="control363" class="col-md-10">
                    <div id="Div4" data-datafield="SFZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                    </div>
                </div>
            </div>
                <div class="row">
                    <div id="title361" class="col-md-2">
                        <span id="Label342" data-type="SheetLabel" data-datafield="SQD" style="">申请单资料</span>
                    </div>
                    <div id="control361" class="col-md-10">
                        <div id="Div13" data-datafield="SQD" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
               
                <div class="row">
                    <div id="title367" class="col-md-2">
                        <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
                    </div>
                    <div id="control367" class="col-md-10">
                        <div id="Div15" data-datafield="JSZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                
                <div class="row">
                    <div id="title375" class="col-md-2">
                        <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
                    </div>
                    <div id="control375" class="col-md-10">
                        <div id="Div19" data-datafield="ZX" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                 <div class="row">
                    <div id="title3871" class="col-md-2">
                        <span id="Label3551" data-type="SheetLabel" data-datafield="KHKKSQS" style="">客户扣款授权书</span>
                    </div>
                    <div id="control3871" class="col-md-10">
                        <div id="Div101" data-datafield="KHKKSQS" data-type="SheetAttachment" style=""   data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
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
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
            <label id="Label2" data-en_us="Sheet information">审核信息</label>
        </div>
        <div class="divContent" id="div20">
            <div class="row tableContent">
                <div id="Div21" class="col-md-2">
                    <span id="Span1" data-type="SheetLabel" data-datafield="CSYJ" style="">运营预审意见</span>
                </div>
                <div id="Div22" class="col-md-10">
                    <div data-datafield="yyysshzt" data-type="SheetRadioButtonList" id="Div23" class="" style="" data-defaultitems="核准;拒绝;"></div>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title399" class="col-md-2">
                    <span id="Label361" data-type="SheetLabel" data-datafield="CSYJ" style="">运营初审意见</span>
                </div>
                <div id="control399" class="col-md-10">
                    <div data-datafield="yycsshzt" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultitems="核准;拒绝;"></div>

                </div>
            </div>
            <div class="row tableContent">
                <div id="Div24" class="col-md-2">
                    <span id="Span2" data-type="SheetLabel" data-datafield="ZSYJj" style="">运营终审意见</span>
                </div>
                <div id="Div25" class="col-md-10">
                    <div data-datafield="yyzsshzt" data-type="SheetRadioButtonList" id="Div26" class="" style="" data-defaultitems="核准;拒绝;"></div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
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
                $(ctrl1).css({ "margin-bottom": "8px", "border-bottom": "1px solid #ccc" });
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
                $(ctrl1).css({ "margin-bottom": "0px", "border-bottom": "0px solid #ccc" });
            }
        }
        $(function () {
            //getDownLoadURL();
            //viewer = new Viewer(document.getElementById('ulfjll'), { url: 'data-original' });
        })
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
            if (Activity == 'Activity48') {
                if (IsMobile) {
                    $("#ctl64724").parent().addClass("chkbank");
                    $("#ctl875733").parent().addClass("chkbank");
                }
                $("#divattachment").hide();
                $("#divattachmentinfo").hide();
            }
            //共借人时候机构贷
            if ($("#Control27").val() && $("#Control27").val() != "") {
                $("#main-navbar").find("h3").html("个人汽车贷款申请<span style=\"color:red;\">(公牌)</span>");
            }
            //风控审核结果转换
            var arrrsrq = [];
            arrrsrq['localreject'] = "东正本地规则<span style=\"color:red;\">拒绝</span>";
            arrrsrq['cloudaccept'] = "云端规则<span style=\"color:red;\">通过</span>";
            arrrsrq['cloudreject'] = "云端规则<span style=\"color:red;\">拒绝</span>";
            arrrsrq['cloudmanual'] = "云端规则返回<span style=\"color:red;\">转人工</span>";
            arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
            $("#control421").find("span").html(arrrsrq[$("#control421").find("span").html()]);
            hideFIrow();
            getmsg();
            //添加留言
            $('#addmsga').on('click', function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
        }
        // 保存事件
        $.MvcSheet.SaveAction.Save = function () {
            // this.Action  // 获取当前按钮名称
            alert($.MvcSheetUI.SheetInfo.InstanceId);
            $.MvcSheetUI.SetControlValue("CJH", $.MvcSheetUI.SheetInfo.InstanceId);
        }
    </script>
</asp:Content>
