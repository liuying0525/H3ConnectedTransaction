<%@ Page Language="C#" AutoEventWireup="true" CodeFile="weekPlanSheet.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.weekPlanSheet" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
<script type="text/javascript">

</script>
<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">周工作计划</label>
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
		<div class="divContent1" id="divSheet">
			<div class="row">
				<div id="title1" class="col-md-2">
					<span id="Label11" data-type="SheetLabel" data-datafield="groupName" style="">小组</span>
				</div>
				<div id="control1" class="col-md-4">
					
				<select data-datafield="groupName" data-type="SheetDropDownList" id="ctl533339" class="" style="" data-defaultitems="开发;项目;核心系统;数据"></select></div>
				<div id="title2" class="col-md-2">
					<span id="Label12" data-type="SheetLabel" data-datafield="bh" style="">编号</span>
				</div>
				<div id="control2" class="col-md-4">
					<input id="Control12" type="text" data-datafield="bh" data-type="SheetTextBox" style="" class="">
				</div>
			</div>
			<div class="row">
				<div id="title3" class="col-md-2">
					<span id="Label13" data-type="SheetLabel" data-datafield="kssj" style="">开始时间</span>
				</div>
				<div id="control3" class="col-md-4">
					<input id="Control13" type="text" data-datafield="kssj" data-type="SheetTime" style="" class="">
				</div>
				<div id="title4" class="col-md-2">
					<span id="Label14" data-type="SheetLabel" data-datafield="jssj" style="">结束日期</span>
				</div>
				<div id="control4" class="col-md-4">
					<input id="Control14" type="text" data-datafield="jssj" data-type="SheetTime" style="">
				</div>
			</div>
			<div class="row tableContent" style="overflow:visible">
				<div id="control51" class="col-md-10" style="width: 100%;padding-bottom: 0px; margin-bottom: 0px;">
					<table id="Control15" data-datafield="weekPlanDetail" data-type="SheetGridView" class="SheetGridView" style="width: 100%;">
						<tbody>
							
							<tr class="header">
								<td id="Control15_SerialNo" class="rowSerialNo" style="width: 5%;">
序号								</td>
								<td id="Control15_Header3" data-datafield="weekPlanDetail.xm" class="" style="width: 15%;">
									<label id="Control15_Label3" data-datafield="weekPlanDetail.xm" data-type="SheetLabel" style="" class="">项目</label>
								</td>
								<td id="Control15_Header4" data-datafield="weekPlanDetail.gznr" class="" style="width: 30%;">
									<label id="Control15_Label4" data-datafield="weekPlanDetail.gznr" data-type="SheetLabel" style="">工作内容</label>
								</td>
								<td id="Control15_Header5" data-datafield="weekPlanDetail.zxr" class="" style="width: 15%;">
									<label id="Control15_Label5" data-datafield="weekPlanDetail.zxr" data-type="SheetLabel" style="" class="">执行人</label>
								</td>
								<td id="Control15_Header6" data-datafield="weekPlanDetail.wcqk" class="" style="width: 15%;">
									<label id="Control15_Label6" data-datafield="weekPlanDetail.wcqk" data-type="SheetLabel" style="">完成情况</label>
								</td>
								<td id="Control15_Header7" data-datafield="weekPlanDetail.sm" class="" style="width: 27%;">
									<label id="Control15_Label7" data-datafield="weekPlanDetail.sm" data-type="SheetLabel" style="" class="">说明</label>
								</td>
								<td class="rowOption">
删除								</td>
							</tr>
							<tr class="template">
								<td id="Control15_Option" class="rowOption">
								</td>
								<td data-datafield="weekPlanDetail.xm" class="" style="width: 0%;">
									
								<select data-datafield="weekPlanDetail.xm" data-type="SheetDropDownList" id="ctl744545" class="" style="" data-masterdatacategory="IT项目"></select></td>
								<td data-datafield="weekPlanDetail.gznr">
									<textarea id="Control15_ctl4" data-datafield="weekPlanDetail.gznr" data-type="SheetRichTextBox" style="height: 50%;" class="">									</textarea>
								</td>
								<td data-datafield="weekPlanDetail.zxr" class="">
									<div id="Control15_ctl5" data-datafield="weekPlanDetail.zxr" data-type="SheetUser" style="width: 100%;" class=""></div>
								</td>
								<td data-datafield="weekPlanDetail.wcqk">
									<select data-datafield="weekPlanDetail.wcqk" data-type="SheetDropDownList" id="ctl494959" class="" style="" data-defaultitems="已完成;进行中" data-displayemptyitem="true">
									</select>
								</td>
								<td data-datafield="weekPlanDetail.sm">
									<textarea id="Control15_ctl7" data-datafield="weekPlanDetail.sm" data-type="SheetRichTextBox" style="height: 90%;" class="">									</textarea>
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
								<td data-datafield="weekPlanDetail.xm">
								</td>
								<td data-datafield="weekPlanDetail.gznr">
								</td>
								<td data-datafield="weekPlanDetail.zxr">
								</td>
								<td data-datafield="weekPlanDetail.wcqk">
								</td>
								<td data-datafield="weekPlanDetail.sm">
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