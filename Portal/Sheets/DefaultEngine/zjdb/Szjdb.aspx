<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Szjdb.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.Szjdb" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
<script type="text/javascript">

</script>
<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">资金调拨</label>
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
					<label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style=""></label>
				</div>
			</div>
		</div>
		<div class="nav-icon fa  fa-chevron-right bannerTitle">
			<label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
		</div>
		<div class="divContent" id="divSheet">
			<div class="row">
				<div id="title1" class="col-md-2">
					<span id="Label11" data-type="SheetLabel" data-datafield="hkxz" style="">划款性质</span>
				</div>
				<div id="control1" class="col-md-4">
					<select data-datafield="hkxz" data-type="SheetDropDownList" id="ctl163766" class="" style="" data-masterdatacategory="划款性质">
					</select>
				</div>
				<div id="title2" class="col-md-2">
					<span id="Label12" data-type="SheetLabel" data-datafield="hkxz_note" style="">划款内容</span>
				</div>
				<div id="control2" class="col-md-4">
					<input id="Control12" type="text" data-datafield="hkxz_note" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row tableContent">
				<div id="title3" class="col-md-2">
					<span id="Label13" data-type="SheetLabel" data-datafield="zjdb_table" style="">划款表单</span>
				</div>
				<div id="control3" class="col-md-10">
					<table id="Control13" data-datafield="zjdb_table" data-type="SheetGridView" class="SheetGridView">
						<tbody>
							
							<tr class="header">
								<td id="Control13_SerialNo" class="rowSerialNo">
序号								</td>
								<td id="Control13_Header3" data-datafield="zjdb_table.fk_date" class="" style="width: 110px;">
									<label id="Control13_Label3" data-datafield="zjdb_table.fk_date" data-type="SheetLabel" style="" class="">付款日期</label>
								</td>
								<td id="Control13_Header4" data-datafield="zjdb_table.fk_company">
									<label id="Control13_Label4" data-datafield="zjdb_table.fk_company" data-type="SheetLabel" style="">付款人户名</label>
								</td>
								<td id="Control13_Header5" data-datafield="zjdb_table.fk_bank">
									<label id="Control13_Label5" data-datafield="zjdb_table.fk_bank" data-type="SheetLabel" style="">付款人开户行</label>
								</td>
								<td id="Control13_Header6" data-datafield="zjdb_table.fk_account">
									<label id="Control13_Label6" data-datafield="zjdb_table.fk_account" data-type="SheetLabel" style="">付款人账号</label>
								</td>
								<td id="Control13_Header7" data-datafield="zjdb_table.sk_user_label">
									<label id="Control13_Label7" data-datafield="zjdb_table.sk_user_label" data-type="SheetLabel" style="">收款人户名</label>
								</td>
								<td id="Control13_Header8" data-datafield="zjdb_table.sk_bank">
									<label id="Control13_Label8" data-datafield="zjdb_table.sk_bank" data-type="SheetLabel" style="">收款人开户行</label>
								</td>
								<td id="Control13_Header9" data-datafield="zjdb_table.sk_account">
									<label id="Control13_Label9" data-datafield="zjdb_table.sk_account" data-type="SheetLabel" style="" class="">收款人账号</label>
								</td>
								<td id="Control13_Header10" data-datafield="zjdb_table.money_type">
									<label id="Control13_Label10" data-datafield="zjdb_table.money_type" data-type="SheetLabel" style="width: 80px;" class="">币种</label>
								</td>
								<td id="Control13_Header11" data-datafield="zjdb_table.hk_money">
									<label id="Control13_Label11" data-datafield="zjdb_table.hk_money" data-type="SheetLabel" style="">转账金额</label>
								</td>
								<td id="Control13_Header12" data-datafield="zjdb_table.note">
									<label id="Control13_Label12" data-datafield="zjdb_table.note" data-type="SheetLabel" style="">备注</label>
								</td>
								<td class="rowOption">
删除								</td>
							</tr>
							<tr class="template">
								<td id="Control13_Option" class="rowOption">
								</td>
								<td data-datafield="zjdb_table.fk_date">
									<input type="text" data-datafield="zjdb_table.fk_date" data-type="SheetTime" id="ctl519857" class="" style="">
								</td>
								<td data-datafield="zjdb_table.fk_company">
									<input id="Control13_ctl4" type="text" data-datafield="zjdb_table.fk_company" data-type="SheetTextBox" style="" class="">
								</td>
								<td data-datafield="zjdb_table.fk_bank">
									<input id="Control13_ctl5" type="text" data-datafield="zjdb_table.fk_bank" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.fk_account">
									<input id="Control13_ctl6" type="text" data-datafield="zjdb_table.fk_account" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.sk_user_label">
									<input id="Control13_ctl7" type="text" data-datafield="zjdb_table.sk_user_label" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.sk_bank">
									<input id="Control13_ctl8" type="text" data-datafield="zjdb_table.sk_bank" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.sk_account">
									<input id="Control13_ctl9" type="text" data-datafield="zjdb_table.sk_account" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.money_type">
									<select data-datafield="zjdb_table.money_type" data-type="SheetDropDownList" id="ctl521133" class="" style="" data-masterdatacategory="币别">
									</select>
								</td>
								<td data-datafield="zjdb_table.hk_money">
									<input id="Control13_ctl11" type="text" data-datafield="zjdb_table.hk_money" data-type="SheetTextBox" style="" class="" data-defaultvalue="0">
								</td>
								<td data-datafield="zjdb_table.note">
									<input id="Control13_ctl12" type="text" data-datafield="zjdb_table.note" data-type="SheetTextBox" style="">
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
								<td class="rowOption">
								</td>
								<td data-datafield="zjdb_table.fk_date">
								</td>
								<td data-datafield="zjdb_table.fk_company">
								</td>
								<td data-datafield="zjdb_table.fk_bank">
								</td>
								<td data-datafield="zjdb_table.fk_account">
								</td>
								<td data-datafield="zjdb_table.sk_user_label">
								</td>
								<td data-datafield="zjdb_table.sk_bank">
								</td>
								<td data-datafield="zjdb_table.sk_account">
								</td>
								<td data-datafield="zjdb_table.money_type">
								</td>
								<td data-datafield="zjdb_table.hk_money" class="">
									<label id="Control13_stat11" data-datafield="zjdb_table.hk_money" data-type="SheetCountLabel" style="" class=""></label>
								</td>
								<td data-datafield="zjdb_table.note">
								</td>
								<td class="rowOption">
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title5" class="col-md-2">
					<span id="Label14" data-type="SheetLabel" data-datafield="fh_remark" style="">复核意见</span>
				</div>
				<div id="control5" class="col-md-10">
					<div id="Control14" data-datafield="fh_remark" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title7" class="col-md-2">
					<span id="Label15" data-type="SheetLabel" data-datafield="fh_remark2" style="">资金部总经理意见</span>
				</div>
				<div id="control7" class="col-md-10">
					<div id="Control15" data-datafield="fh_remark2" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title9" class="col-md-2">
					<span id="Label16" data-type="SheetLabel" data-datafield="manager_finc_remark" style="">分管资金副总意见</span>
				</div>
				<div id="control9" class="col-md-10">
					<div id="Control16" data-datafield="manager_finc_remark" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title11" class="col-md-2">
					<span id="Label17" data-type="SheetLabel" data-datafield="gen_manager_remark" style="">总经理意见</span>
				</div>
				<div id="control11" class="col-md-10">
					<div id="Control17" data-datafield="gen_manager_remark" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row">
				<div id="title13" class="col-md-2">
					<span id="Label18" data-type="SheetLabel" data-datafield="pass_round" style="">传阅</span>
				</div>
				<div id="control13" class="col-md-4">
					<div id="Control18" data-datafield="pass_round" data-type="SheetUser" style=""></div>
				</div>
				<div id="space14" class="col-md-2">
				</div>
				<div id="spaceControl14" class="col-md-4">
				</div>
			</div>
			<div class="row">
				<div id="title15" class="col-md-2">
					<span id="Label19" data-type="SheetLabel" data-datafield="attachment" style="">附件</span>
				</div>
				<div id="control15" class="col-md-10">
					<div id="Control19" data-datafield="attachment" data-type="SheetAttachment" style=""></div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>