<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SorganizationImport.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SorganizationImport" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
<script type="text/javascript">

</script>
<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">组织架构导入</label>
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
		<div class="divContent" id="divSheet">
			<div class="row tableContent">
				<div id="title1" class="col-md-2">
					<span id="Label11" data-type="SheetLabel" data-datafield="ryxxbImport" style="">人员信息表</span>
				</div>
				<div id="control1" class="col-md-10">
					<table id="Control11" data-datafield="ryxxbImport" data-type="SheetGridView" class="SheetGridView" data-displayimport="true" data-displayexport="true">
						<tbody>
							
							<tr class="header">
								<td id="Control11_SerialNo" class="rowSerialNo">
序号								</td>
								<td id="Control11_Header3" data-datafield="ryxxbImport.bm">
									<label id="Control11_Label3" data-datafield="ryxxbImport.bm" data-type="SheetLabel" style="">部门</label>
								</td>
								<td id="Control11_Header4" data-datafield="ryxxbImport.xm">
									<label id="Control11_Label4" data-datafield="ryxxbImport.xm" data-type="SheetLabel" style="">姓名</label>
								</td>
								<td id="Control11_Header5" data-datafield="ryxxbImport.zh">
									<label id="Control11_Label5" data-datafield="ryxxbImport.zh" data-type="SheetLabel" style="">账号</label>
								</td>
								<td id="Control11_Header6" data-datafield="ryxxbImport.dh">
									<label id="Control11_Label6" data-datafield="ryxxbImport.dh" data-type="SheetLabel" style="">电话</label>
								</td>
								<td class="rowOption">
删除								</td>
							</tr>
							<tr class="template">
								<td id="Control11_Option" class="rowOption">
								</td>
								<td data-datafield="ryxxbImport.bm">
									<input id="Control11_ctl3" type="text" data-datafield="ryxxbImport.bm" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="ryxxbImport.xm">
									<input id="Control11_ctl4" type="text" data-datafield="ryxxbImport.xm" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="ryxxbImport.zh">
									<input id="Control11_ctl5" type="text" data-datafield="ryxxbImport.zh" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="ryxxbImport.dh">
									<input id="Control11_ctl6" type="text" data-datafield="ryxxbImport.dh" data-type="SheetTextBox" style="">
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
								<td data-datafield="ryxxbImport.bm">
								</td>
								<td data-datafield="ryxxbImport.xm">
								</td>
								<td data-datafield="ryxxbImport.zh">
								</td>
								<td data-datafield="ryxxbImport.dh">
								</td>
								<td class="rowOption">
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
	</div>
</asp:Content>