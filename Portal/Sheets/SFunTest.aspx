<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SFunTest.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SFunTest" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
	<script type="text/javascript">

</script>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">功能测试</label>
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
			<div class="row">
				<div id="title1" class="col-md-2">
					<span id="Label11" data-type="SheetLabel" data-datafield="funname" style="">测试功能</span>
				</div>
				<div id="control1" class="col-md-4">
					<input id="Control11" type="text" data-datafield="funname" data-type="SheetTextBox" style="">
				</div>
				<div id="title2" class="col-md-2">
					<span id="Label12" data-type="SheetLabel" data-datafield="num" style="">数字</span>
				</div>
				<div id="control2" class="col-md-4">
					<input id="Control12" type="text" data-datafield="num" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row">
				<div id="title3" class="col-md-2">
					<span id="Label13" data-type="SheetLabel" data-datafield="instanceid" style="">实例ID</span>
				</div>
				<div id="control3" class="col-md-4">
					<input id="Control13" type="text" data-datafield="instanceid" data-type="SheetTextBox" style="">
				</div>
				<div id="space4" class="col-md-2">
				</div>
				<div id="spaceControl4" class="col-md-4">
				</div>
			</div>
			<div class="row tableContent">
				<div id="title5" class="col-md-2">
					<span id="Label14" data-type="SheetLabel" data-datafield="subtable" style="">子表</span>
				</div>
				<div id="control5" class="col-md-10">
					<table id="Control14" data-datafield="subtable" data-type="SheetGridView" class="SheetGridView">
						<tbody>
							
							<tr class="header">
								<td id="Control14_SerialNo" class="rowSerialNo">
序号								</td>
								<td id="Control14_Header3" data-datafield="subtable.id">
									<label id="Control14_Label3" data-datafield="subtable.id" data-type="SheetLabel" style="">编号</label>
								</td>
								<td id="Control14_Header4" data-datafield="subtable.mc">
									<label id="Control14_Label4" data-datafield="subtable.mc" data-type="SheetLabel" style="">名称</label>
								</td>
								<td class="rowOption">
删除								</td>
							</tr>
							<tr class="template">
								<td id="Control14_Option" class="rowOption">
								</td>
								<td data-datafield="subtable.id">
									<input id="Control14_ctl3" type="text" data-datafield="subtable.id" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="subtable.mc">
									<input id="Control14_ctl4" type="text" data-datafield="subtable.mc" data-type="SheetTextBox" style="">
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
								<td data-datafield="subtable.id">
								</td>
								<td data-datafield="subtable.mc">
								</td>
								<td class="rowOption">
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
			<div class="row">
				<div id="title7" class="col-md-2">
					<span id="Label15" data-type="SheetLabel" data-datafield="fj" style="">附件</span>
				</div>
				<div id="control7" class="col-md-10">
					<div id="Control15" data-datafield="fj" data-type="SheetAttachment" style="">
						
					</div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title9" class="col-md-2">
					<span id="Label16" data-type="SheetLabel" data-datafield="sp1" style="">审批1</span>
				</div>
				<div id="control9" class="col-md-10">
					<div id="Control16" data-datafield="sp1" data-type="SheetComment" style="">
						
					</div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title11" class="col-md-2">
					<span id="Label17" data-type="SheetLabel" data-datafield="sp2" style="">审批2</span>
				</div>
				<div id="control11" class="col-md-10">
					<div id="Control17" data-datafield="sp2" data-type="SheetComment" style="">
						
					</div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title13" class="col-md-2">
					<span id="Label18" data-type="SheetLabel" data-datafield="sp3" style="">审批3</span>
				</div>
				<div id="control13" class="col-md-10">
					<div id="Control18" data-datafield="sp3" data-type="SheetComment" style="">
						
					</div>
				</div>
			</div>
			<div class="row">
				<div id="title15" class="col-md-2">
					<span id="Label19" data-type="SheetLabel" data-datafield="cyz1" style="">参与者1</span>
				</div>
				<div id="control15" class="col-md-4">
					<div id="Control19" data-datafield="cyz1" data-type="SheetUser" style="">
						
					</div>
				</div>
				<div id="title16" class="col-md-2">
					<span id="Label20" data-type="SheetLabel" data-datafield="cyz2" style="">参与者2</span>
				</div>
				<div id="control16" class="col-md-4">
					<div id="Control20" data-datafield="cyz2" data-type="SheetUser" style="">
						
					</div>
				</div>
			</div>
			<div class="row">
				<div id="title17" class="col-md-2">
					<span id="Label21" data-type="SheetLabel" data-datafield="cyz3" style="">参与者3</span>
				</div>
				<div id="control17" class="col-md-4">
					<div id="Control21" data-datafield="cyz3" data-type="SheetUser" style="">
						
					</div>
				</div>
				<div id="space18" class="col-md-2">
				</div>
				<div id="spaceControl18" class="col-md-4">
				</div>
			</div>
		</div>
	</div>
</asp:Content>