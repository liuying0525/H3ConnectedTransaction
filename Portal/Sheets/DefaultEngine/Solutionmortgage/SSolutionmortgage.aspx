<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SSolutionmortgage.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SSolutionmortgage" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <script src="/Portal/WFRes/_Scripts/PriintEnterMortgage.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
<script type="text/javascript">
    $.MvcSheet.Loaded = function () {          
        var applyNo = $.MvcSheetUI.SheetInfo.BizObject.DataItems["ApplyNo"].V;
        $("#btnPrint").click(function () {
            $.ajax({
                url: "/Portal/ajax/PrintWord.ashx",
                data: { 'ApplyNo': applyNo },
                type: "post",
                async: true,
                dataType: "json",
                success: function(result) {
                    debugger;
                }
            });
        });
        //$("[data-attachment]").attr("href", "/Portal/Sheets/DefaultEngine/Mortgage/JYB.html?" + $.MvcSheetUI.SheetInfo.ApplyNo);
    }
</script>
<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">解押流程</label>
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
				<label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style=""></label>
			</div>
			<div id="divOriginateDateTitle" class="col-md-2">
				<label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
			</div>
			<div id="divOriginateDate" class="col-md-4">
				<label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style=""></label>
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
				<div id="divSequenceNoTitle" class="col-md-2">
					<label id="lblSequenceNoTitle" data-type="SheetLabel" data-datafield="SequenceNo" data-en_us="SequenceNo" data-bindtype="OnlyVisiable" style="">流水号</label>
				</div>
				<div id="divSequenceNo" class="col-md-4">
					<label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style=""></label>
				</div>
			</div>
		
		<div class="row">
				<div id="title1" class="col-md-2">
					<span id="Label11" data-type="SheetLabel" data-datafield="Province" style="">所在省份</span>
				</div>
				<div id="control1" class="col-md-4">
					<input id="Control11" type="text" data-datafield="Province" data-type="SheetTextBox" style="">
				</div>
				<div id="title2" class="col-md-2">
					<span id="Label12" data-type="SheetLabel" data-datafield="City" style="">所在城市</span>
				</div>
				<div id="control2" class="col-md-4">
					<input id="Control12" type="text" data-datafield="City" data-type="SheetTextBox" style="">
				</div>
			</div>
		</div>
		<div class="nav-icon fa  fa-chevron-right bannerTitle">
			<label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
		</div>
		<div class="divContent" id="divSheet">
			
			<div class="row">
				<div id="title3" class="col-md-2">
					<span id="Label13" data-type="SheetLabel" data-datafield="ApplyType" style="">申请类型</span>
				</div>
				<div id="control3" class="col-md-4">
					<input id="Control13" type="text" data-datafield="ApplyType" data-type="SheetTextBox" style="">
				</div>
				<div id="title4" class="col-md-2">
					
				</div>
				<div id="control4" class="col-md-4">
					
				</div>				
			</div>
            <div class="row tableContent">
                <div id="title43" class="col-md-2">
                    <span id="Label46" data-type="SheetLabel" data-datafield="GJPeoples" style="">个人</span>
                </div>
                <div id="control43" class="col-md-10">
                    <table id="Control46" data-datafield="GJPeoples" data-type="SheetGridView" class="SheetGridView">
                        <tbody>

                            <tr class="header">
                                <td id="Control46_SerialNo" class="rowSerialNo">
                                    序号
                                </td>
                                <td id="Control46_Header3" data-datafield="GJPeoples.PeopleType">
                                    <label id="Control46_Label3" data-datafield="GJPeoples.PeopleType" data-type="SheetLabel" style="">人员类型</label>
                                </td>
                                <td id="Control46_Header4" data-datafield="GJPeoples.People">
                                    <label id="Control46_Label4" data-datafield="GJPeoples.People" data-type="SheetLabel" style="">姓名</label>
                                </td>
                                <td id="Control46_Header5" data-datafield="GJPeoples.IdType">
                                    <label id="Control46_Label5" data-datafield="GJPeoples.IdType" data-type="SheetLabel" style="">证件类型</label>
                                </td>
                                <td id="Control46_Header6" data-datafield="GJPeoples.ID">
                                    <label id="Control46_Label6" data-datafield="GJPeoples.ID" data-type="SheetLabel" style="">证件号</label>
                                </td>
                                <td class="rowOption">
                                    删除
                                </td>
                            </tr>
                            <tr class="template">
                                <td id="Control46_Option" class="rowOption"></td>
                                <td data-datafield="GJPeoples.PeopleType">
                                    <input id="Control46_ctl3" type="text" data-datafield="GJPeoples.PeopleType" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="GJPeoples.People">
                                    <input id="Control46_ctl4" type="text" data-datafield="GJPeoples.People" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="GJPeoples.IdType">
                                    <input id="Control46_ctl5" type="text" data-datafield="GJPeoples.IdType" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="GJPeoples.ID">
                                    <input id="Control46_ctl6" type="text" data-datafield="GJPeoples.ID" data-type="SheetTextBox" style="">
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
                                <td data-datafield="GJPeoples.PeopleType"></td>
                                <td data-datafield="GJPeoples.People"></td>
                                <td data-datafield="GJPeoples.IdType"></td>
                                <td data-datafield="GJPeoples.ID"></td>
                                <td class="rowOption"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
			<div class="row">
				<div id="title10" class="col-md-2">
					<span id="Label20" data-type="SheetLabel" data-datafield="Assets" style="">资产状况</span>
				</div>
				<div id="control10" class="col-md-4">
					<input id="Control20" type="text" data-datafield="Assets" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title11" class="col-md-2">
					<span id="Label21" data-type="SheetLabel" data-datafield="Factory" style="">制造商</span>
				</div>
				<div id="control11" class="col-md-4">
					<input id="Control21" type="text" data-datafield="Factory" data-type="SheetTextBox" style="">
				</div>
				<div id="title12" class="col-md-2">
					<span id="Label22" data-type="SheetLabel" data-datafield="CarType" style="">车型</span>
				</div>
				<div id="control12" class="col-md-4">
					<input id="Control22" type="text" data-datafield="CarType" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title13" class="col-md-2">
					<span id="Label23" data-type="SheetLabel" data-datafield="NewCarPrice" style="">新车指导价</span>
				</div>
				<div id="control13" class="col-md-4">
					<input id="Control23" type="text" data-datafield="NewCarPrice" data-type="SheetTextBox" style="">
				</div>
				<div id="title14" class="col-md-2">
					<span id="Label24" data-type="SheetLabel" data-datafield="AssetsPrice" style="">资产价格</span>
				</div>
				<div id="control14" class="col-md-4">
					<input id="Control24" type="text" data-datafield="AssetsPrice" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title15" class="col-md-2">
					<span id="Label25" data-type="SheetLabel" data-datafield="MotorType" style="">发动机号码</span>
				</div>
				<div id="control15" class="col-md-4">
					<input id="Control25" type="text" data-datafield="MotorType" data-type="SheetTextBox" style="">
				</div>
				<div id="title16" class="col-md-2">
					<span id="Label26" data-type="SheetLabel" data-datafield="FrameNumber" style="">车架号</span>
				</div>
				<div id="control16" class="col-md-4">
					<input id="Control26" type="text" data-datafield="FrameNumber" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title17" class="col-md-2">
					<span id="Label27" data-type="SheetLabel" data-datafield="ProductClass" style="">产品组</span>
				</div>
				<div id="control17" class="col-md-4">
					<input id="Control27" type="text" data-datafield="ProductClass" data-type="SheetTextBox" style="">
				</div>
				<div id="title18" class="col-md-2">
					<span id="Label28" data-type="SheetLabel" data-datafield="ProductType" style="">产品类型</span>
				</div>
				<div id="control18" class="col-md-4">
					<input id="Control28" type="text" data-datafield="ProductType" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title19" class="col-md-2">
					<span id="Label29" data-type="SheetLabel" data-datafield="DKMoney" style="">贷款金额</span>
				</div>
				<div id="control19" class="col-md-4">
					<input id="Control29" type="text" data-datafield="DKMoney" data-type="SheetTextBox" style="">
				</div>
				<div id="title20" class="col-md-2">
					<span id="Label30" data-type="SheetLabel" data-datafield="FJMoney" style="">附加费</span>
				</div>
				<div id="control20" class="col-md-4">
					<input id="Control30" type="text" data-datafield="FJMoney" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title21" class="col-md-2">
					<span id="Label31" data-type="SheetLabel" data-datafield="SFMoney" style="">首付金额</span>
				</div>
				<div id="control21" class="col-md-4">
					<input id="Control31" type="text" data-datafield="SFMoney" data-type="SheetTextBox" style="">
				</div>
				<div id="title22" class="col-md-2">
					<span id="Label32" data-type="SheetLabel" data-datafield="IsSelf" style="">是否自办</span>
				</div>
				<div id="control22" class="col-md-4">
					<input id="Control32" type="text" data-datafield="IsSelf" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title23" class="col-md-2">
					<span id="Label33" data-type="SheetLabel" data-datafield="LocalDo" style="">本地办理</span>
				</div>
				<div id="control23" class="col-md-4">
					<input id="Control33" type="text" data-datafield="LocalDo" data-type="SheetTextBox" style="">
				</div>
				<div id="title24" class="col-md-2">
					<span id="Label34" data-type="SheetLabel" data-datafield="SelectShop" style="">办理店选择</span>
				</div>
				<div id="control24" class="col-md-4">
					<input id="Control34" type="text" data-datafield="SelectShop" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title25" class="col-md-2">
					<span id="Label35" data-type="SheetLabel" data-datafield="SPProvince" style="">上牌抵押省份</span>
				</div>
				<div id="control25" class="col-md-4">
					<input id="Control35" type="text" data-datafield="SPProvince" data-type="SheetTextBox" style="">
				</div>
				<div id="title26" class="col-md-2">
					<span id="Label36" data-type="SheetLabel" data-datafield="SPCity" style="">上牌抵押城市</span>
				</div>
				<div id="control26" class="col-md-4">
					<input id="Control36" type="text" data-datafield="SPCity" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title27" class="col-md-2">
					<span id="Label37" data-type="SheetLabel" data-datafield="Certificate" style="">合格证</span>
				</div>
				<div id="control27" class="col-md-10">
					<div id="Control37" data-datafield="Certificate" data-type="SheetAttachment" style=""></div>
				</div>
			</div>
			<div class="row">
				<div id="title29" class="col-md-2">
					<span id="Label38" data-type="SheetLabel" data-datafield="LicenceNo" style="">牌照号</span>
				</div>
				<div id="control29" class="col-md-4">
					<input id="Control38" type="text" data-datafield="LicenceNo" data-type="SheetTextBox" style="">
				</div>
				<div id="space30" class="col-md-2">
				</div>
				<div id="spaceControl30" class="col-md-4">
				</div>
			</div>
			<div class="row">
				<div id="title31" class="col-md-2">
					<span id="Label39" data-type="SheetLabel" data-datafield="OtherFile" style="">其他附件</span>
				</div>
				<div id="control31" class="col-md-10">
					<div id="Control39" data-datafield="OtherFile" data-type="SheetAttachment" style=""></div>
				</div>
			</div>
			<div class="row">
				<div id="title33" class="col-md-2">
					<span id="Label40" data-type="SheetLabel" data-datafield="MortgageFile" style="">抵押资料</span>
				</div>
				<div id="control33" class="col-md-10">
					<div id="Control40" data-datafield="MortgageFile" data-type="SheetAttachment" style=""></div>
				</div>
			</div>
			<div class="row">
				<div id="title35" class="col-md-2">
					<span id="Label41" data-type="SheetLabel" data-datafield="SPType" style="">审批</span>
				</div>
				<div id="control35" class="col-md-4">
					<input id="Control41" type="text" data-datafield="SPType" data-type="SheetTextBox" style="">
				</div>
				<div id="space36" class="col-md-2">
				</div>
				<div id="spaceControl36" class="col-md-4">
				</div>
			</div>
			<div class="row tableContent">
				<div id="title37" class="col-md-2">
					<span id="Label42" data-type="SheetLabel" data-datafield="SPAdvise" style="">审批意见</span>
				</div>
				<div id="control37" class="col-md-10">
					<div id="Control42" data-datafield="SPAdvise" data-type="SheetComment" style="" class=""></div>
				</div>
			</div>
			<div class="row">
				<div id="title39" class="col-md-2">
					<span id="Label43" data-type="SheetLabel" data-datafield="ExpressNumber" style="">快递单号</span>
				</div>
				<div id="control39" class="col-md-4">
					<input id="Control43" type="text" data-datafield="ExpressNumber" data-type="SheetTextBox" style="">
				</div>
				<div id="title40" class="col-md-2">
					<span id="Label44" data-type="SheetLabel" data-datafield="RchivingResults" style="">归档结果</span>
				</div>
				<div id="control40" class="col-md-4">
					<input id="Control44" type="text" data-datafield="RchivingResults" data-type="SheetTextBox" style="">
				</div>
			</div>
	    	<div class="row">
				<div id="title390" class="col-md-2" style="display: none;">
					<span id="Label43" data-type="SheetLabel" data-datafield="ApplyNo" style="">申请号</span>
				</div>
		        <div id="control9" class="col-md-4">
		            <select name="select2" id="select2" style="width: 200px" onchange="">
		                <option value="" selected="selected">《——请选择——》</option>
		            </select>
		        </div>
                <div id="div605744" class="col-md-8">
                    <%--<a href="" data-attachment="" target="_blank" class="printHidden btn btn-outline btn-lg" style="width: 100%; border: 1px dashed rgb(221, 221, 221);">打印</a>--%>
                    <a onclick="jumpToEidtDoc('NTKO', false);" target="_blank" class="printHidden btn btn-outline btn-lg" style="width: 100%; border: 1px dashed rgb(221, 221, 221);">打印</a>
                </div>
            </div>
		</div>
	</div>
    <script>
        $.MvcSheet.Loaded = function () {
            SheetLoaded();
        }
    </script>
</asp:Content>