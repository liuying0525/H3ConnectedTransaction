<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SRetailApp.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SRetailApp" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

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

         #example {
            width: 1100px;
            height: 500px;
            background: white;
            overflow: hidden;
            left: 50%;
            top: 50%;
            transform: translate(-50%,-50%);
            border-radius: 3px;
        }

            #example .modal-body {
                width: 100%;
                height: 400px;
                overflow: auto;
                padding: 10px;
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
                    <input id="applicationNo" type="text" data-datafield="applicationNo" data-type="SheetTextBox" style="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="appTypeName" style="">申请类型</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="appTypeName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="appCreateTime" style="">日期</span>
                </div>
                <div id="control3" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="appCreateTime" data-type="SheetTextBox" style="">
                </div>
                <div id="title4" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="companyName" style="">申请贷款公司</span>
                </div>
                <div id="control4" class="col-md-4">
                    <input id="Control14" type="text" data-datafield="companyName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="financialadvisorName" style="">金融顾问</span>
                </div>
                <div id="control5" class="col-md-4">
                    <input id="Control15" type="text" data-datafield="financialadvisorName" data-type="SheetTextBox" style="">
                </div>
                <div id="title6" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="vehicleTypeName" style="">车辆类型</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control16" type="text" data-datafield="vehicleTypeName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title7" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="provinceName" style="">经销商省</span>
                </div>
                <div id="control7" class="col-md-4">
                    <input id="Control17" type="text" data-datafield="provinceName" data-type="SheetTextBox" style="">
                </div>
                <div id="title8" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="dealercityName" style="">经销商城市</span>
                </div>
                <div id="control8" class="col-md-4">
                    <input id="Control18" type="text" data-datafield="dealercityName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title9" class="col-md-2">
                    <span id="Label19" data-type="SheetLabel" data-datafield="salesPersonName" style="">销售人员</span>
                </div>
                <div id="control9" class="col-md-4">
                    <input id="Control19" type="text" data-datafield="salesPersonName" data-type="SheetTextBox" style="">
                </div>
                <div id="title10" class="col-md-2">
                    <span id="Label20" data-type="SheetLabel" data-datafield="showRoomName" style="">展厅</span>
                </div>
                <div id="control10" class="col-md-4">
                    <input id="Control20" type="text" data-datafield="showRoomName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title11" class="col-md-2">
                    <span id="Label21" data-type="SheetLabel" data-datafield="distributorName" style="">制造商</span>
                </div>
                <div id="control11" class="col-md-4">
                    <input id="Control21" type="text" data-datafield="distributorName" data-type="SheetTextBox" style="">
                </div>
                <div id="title12" class="col-md-2">
                    <span id="Label22" data-type="SheetLabel" data-datafield="ContactNumber" style="">联系方式</span>
                </div>
                <div id="control12" class="col-md-4">
                    <input id="Control22" type="text" data-datafield="ContactNumber" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title13" class="col-md-2">
                    <span id="Label23" data-type="SheetLabel" data-datafield="refCANumber" style="">申请参考号</span>
                </div>
                <div id="control13" class="col-md-4">
                    <input id="Control23" type="text" data-datafield="refCANumber" data-type="SheetTextBox" style="">
                </div>
                <div id="title14" class="col-md-2">
                    <span id="Label24" data-type="SheetLabel" data-datafield="dealerName" style="">经销商</span>
                </div>
                <div id="control14" class="col-md-4">
                    <input id="Control24" type="text" data-datafield="dealerName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title15" class="col-md-2">
                    <span id="Label25" data-type="SheetLabel" data-datafield="userName" style="">账户名</span>
                </div>
                <div id="control15" class="col-md-4">
                    <input id="Control25" type="text" data-datafield="userName" data-type="SheetTextBox" style="">
                </div>
                <div id="title19" class="col-md-2">
                    <span id="Label27" data-type="SheetLabel" data-datafield="applicant_type" style="">申请类型</span>
                </div>
                <div id="control19" class="col-md-4">
                    <input id="Control27" type="text" data-datafield="applicant_type" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title20" class="col-md-2 light">
                    <span id="Label28" data-type="SheetLabel" data-datafield="IS_INACTIVE_IND" style="">是否隐藏共借人</span>
                </div>
                <div id="control20" class="col-md-4 signcolor">
                    <input id="Control28" type="text" data-datafield="IS_INACTIVE_IND" data-type="SheetTextBox" style="">
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
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divSQR',this)">
            <label id="Label5" data-en_us="Sheet information">申请人信息</label>
        </div>
        <div class="divContent" id="divSQR">
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInfo',this)">
                <label id="Label8" data-en_us="Sheet information">基本信息</label>
            </div>
            <div id="divBascSQRInfo">
                <div class="row">
                    <div class="col-md-2">
                        <label data-type="SheetLabel" style="">风控模型评估</label>
                    </div>
                    <div class="col-md-2">
                        <label>评估结果</label>
                        <a href="javascript:void(0);" onclick="rsfkClick();">评估结果</a>
                    </div>
                    <div class="col-md-2">
                         <label>NCIIC</label>
                        <a href="javascript:void(0);" onclick="nciicClick();">NCIIC</a>
                    </div>
                    <div class="col-md-4" id="aclickh">
                        <label>PBOC</label>
                        <a href="javascript:void(0);" onclick="pbocClick('Control29','Control30','ctrlIdCardNo');">申请人</a>
                        <a href="javascript:void(0);" onclick="pbocClick('Control122','Control123','Control124');">共借人</a>
                        <a href="javascript:void(0);" onclick="pbocClick('Control209','Control210','Control211');">担保人</a>
                        
                    </div>
                    <div class="col-md-2" style="position:relative;">
                        <a class="btn" id="rsmanchk" onclick="rsmanchk()">人工查询</a>
                        <span id="searching" style="position:absolute;white-space:nowrap;"></span>
                    </div>
                </div>

                <div class="row" style="display:none;">
			<div id="title421" class="col-md-2">
				<span id="Label380" data-type="SheetLabel" data-datafield="fkResult" style="">风控审核结果</span>
			</div>
			<div id="control421" class="col-md-4">
				<input id="Control380" type="text" data-datafield="fkResult" data-type="SheetTextBox" style="">
                                <a href="#" id="aclick" target="_blank"></a>
			</div>
			
		</div>

                <div class="row">
                    <div id="title21" class="col-md-2 light">
                        <span id="Label29" data-type="SheetLabel" data-datafield="ThaiFirstName" style="">姓名（中文）</span>
                    </div>
                    <div id="control21" class="col-md-4 signcolor">
                        <input id="Control29" type="text" data-datafield="ThaiFirstName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title22" class="col-md-2">
                        <span id="Label30" data-type="SheetLabel" data-datafield="IdCarTypeName" style="">证件类型</span>
                    </div>
                    <div id="control22" class="col-md-4">
                        <input id="Control30" type="text" data-datafield="IdCarTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title23" class="col-md-2">
                        <span id="Label31" data-type="SheetLabel" data-datafield="IdCardNo" style="">证件号码</span>
                    </div>
                    <div id="control23" class="col-md-4">
                        <input id="ctrlIdCardNo" type="text" data-datafield="IdCardNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title24" class="col-md-2 light">
                        <span id="Label32" data-type="SheetLabel" data-datafield="MaritalStatusName" style="">婚姻状况</span>
                    </div>
                    <div id="control24" class="col-md-4 signcolor">
                        <input id="Control32" type="text" data-datafield="MaritalStatusName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title25" class="col-md-2">
                        <span id="Label33" data-type="SheetLabel" data-datafield="genderName" style="">性别</span>
                    </div>
                    <div id="control25" class="col-md-4">
                        <input id="Control33" type="text" data-datafield="genderName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title27" class="col-md-2 light">
                        <span id="Label34" data-type="SheetLabel" data-datafield="HokouName" style="">户口所在地</span>
                    </div>
                    <div id="control27" class="col-md-4 signcolor">
                        <textarea id="Control34" data-datafield="HokouName" data-type="SheetTextBox" style="">					</textarea>
                    </div>
                    <%-- <div id="space26" class="col-md-2">
                        
                    </div>
                    <div id="spaceControl26" class="col-md-4">
                        
                    </div>--%>
                </div>
                <div class="row">
                    <div id="title29" class="col-md-2 light">
                        <span id="Label35" data-type="SheetLabel" data-datafield="DateOfBirth" style="">出生日期</span>
                    </div>
                    <div id="control29" class="col-md-4 signcolor">
                        <input id="Control35" type="text" data-datafield="DateOfBirth" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title30" class="col-md-2">
                        <span id="Label36" data-type="SheetLabel" data-datafield="CitizenshipName" style="">国籍</span>
                    </div>
                    <div id="control30" class="col-md-4">
                        <input id="Control36" type="text" data-datafield="CitizenshipName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title31" class="col-md-2">
                        <span id="Label37" data-type="SheetLabel" data-datafield="IdCardIssueDate" style="">证件发行日</span>
                    </div>
                    <div id="control31" class="col-md-4">
                        <input id="Control37" type="text" data-datafield="IdCardIssueDate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title32" class="col-md-2">
                        <span id="Label38" data-type="SheetLabel" data-datafield="IdCardExpiryDate" style="">证件到期日</span>
                    </div>
                    <div id="control32" class="col-md-4">
                        <input id="Control38" type="text" data-datafield="IdCardExpiryDate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title33" class="col-md-2">
                        <span id="Label39" data-type="SheetLabel" data-datafield="LicenseNo" style="">驾照号码</span>
                    </div>
                    <div id="control33" class="col-md-4">
                        <input id="Control39" type="text" data-datafield="LicenseNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title34" class="col-md-2">
                        <span id="Label40" data-type="SheetLabel" data-datafield="LicenseExpiryDate" style="">驾照到期日</span>
                    </div>
                    <div id="control34" class="col-md-4">
                        <input id="Control40" type="text" data-datafield="LicenseExpiryDate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title35" class="col-md-2">
                        <span id="Label41" data-type="SheetLabel" data-datafield="ThaiTitleName" style="">头衔</span>
                    </div>
                    <div id="control35" class="col-md-4">
                        <input id="Control41" type="text" data-datafield="ThaiTitleName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title36" class="col-md-2">
                        <span id="Label42" data-type="SheetLabel" data-datafield="TitleName" style="">头衔（英文）</span>
                    </div>
                    <div id="control36" class="col-md-4">
                        <input id="Control42" type="text" data-datafield="TitleName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title37" class="col-md-2 light">
                        <span id="Label43" data-type="SheetLabel" data-datafield="DrivingLicenseStatusName" style="">驾照状态</span>
                    </div>
                    <div id="control37" class="col-md-4 signcolor">
                        <input id="Control43" type="text" data-datafield="DrivingLicenseStatusName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title38" class="col-md-2">
                        <span id="Label44" data-type="SheetLabel" data-datafield="EngFirstName" style="">名（英文）</span>
                    </div>
                    <div id="control38" class="col-md-4">
                        <input id="Control44" type="text" data-datafield="EngFirstName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title39" class="col-md-2">
                        <span id="Label45" data-type="SheetLabel" data-datafield="EngMiddleName" style="">中间名字</span>
                    </div>
                    <div id="control39" class="col-md-4">
                        <input id="Control45" type="text" data-datafield="EngMiddleName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title40" class="col-md-2">
                        <span id="Label46" data-type="SheetLabel" data-datafield="EngLastName" style="">姓（英文）</span>
                    </div>
                    <div id="control40" class="col-md-4">
                        <input id="Control46" type="text" data-datafield="EngLastName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title41" class="col-md-2">
                        <span id="Label47" data-type="SheetLabel" data-datafield="FormerName" style="">曾用名</span>
                    </div>
                    <div id="control41" class="col-md-4">
                        <input id="Control47" type="text" data-datafield="FormerName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title42" class="col-md-2">
                        <span id="Label48" data-type="SheetLabel" data-datafield="NationName" style="">民族</span>
                    </div>
                    <div id="control42" class="col-md-4">
                        <input id="Control48" type="text" data-datafield="NationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title43" class="col-md-2">
                        <span id="Label49" data-type="SheetLabel" data-datafield="IssuingAuthority" style="">签发机关</span>
                    </div>
                    <div id="control43" class="col-md-4">
                        <input id="Control49" type="text" data-datafield="IssuingAuthority" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title44" class="col-md-2">
                        <span id="Label50" data-type="SheetLabel" data-datafield="NumberOfDependents" style="">供养人数</span>
                    </div>
                    <div id="control44" class="col-md-4">
                        <input id="Control50" type="text" data-datafield="NumberOfDependents" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title45" class="col-md-2">
                        <span id="Label51" data-type="SheetLabel" data-datafield="HouseOwnerName" style="">房产所有人</span>
                    </div>
                    <div id="control45" class="col-md-4">
                        <input id="Control51" type="text" data-datafield="HouseOwnerName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title46" class="col-md-2">
                        <span id="Label52" data-type="SheetLabel" data-datafield="NoOfFamilyMembers" style="">家庭人数</span>
                    </div>
                    <div id="control46" class="col-md-4">
                        <input id="Control52" type="text" data-datafield="NoOfFamilyMembers" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title47" class="col-md-2">
                        <span id="Label53" data-type="SheetLabel" data-datafield="ChildrenFlag" style="">是否是子女</span>
                    </div>
                    <div id="control47" class="col-md-4">
                        <input id="Control53" type="text" data-datafield="ChildrenFlag" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title48" class="col-md-2">
                        <span id="Label54" data-type="SheetLabel" data-datafield="EmploymentTypeName" style="">雇员类型</span>
                    </div>
                    <div id="control48" class="col-md-4">
                        <input id="Control54" type="text" data-datafield="EmploymentTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title49" class="col-md-2">
                        <span id="Label55" data-type="SheetLabel" data-datafield="EmailAddress" style="">邮箱地址</span>
                    </div>
                    <div id="control49" class="col-md-4">
                        <input id="Control55" type="text" data-datafield="EmailAddress" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title50" class="col-md-2 light">
                        <span id="Label56" data-type="SheetLabel" data-datafield="EducationName" style="">教育程度</span>
                    </div>
                    <div id="control50" class="col-md-4 signcolor">
                        <input id="Control56" type="text" data-datafield="EducationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title51" class="col-md-2">
                        <span id="Label57" data-type="SheetLabel" data-datafield="IndustryTypeName" style="">行业类型</span>
                    </div>
                    <div id="control51" class="col-md-4">
                        <input id="Control57" type="text" data-datafield="IndustryTypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title52" class="col-md-2">
                        <span id="Label58" data-type="SheetLabel" data-datafield="IndustrySubTypeName" style="">行业子类型</span>
                    </div>
                    <div id="control52" class="col-md-4">
                        <input id="Control58" type="text" data-datafield="IndustrySubTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title53" class="col-md-2">
                        <span id="Label59" data-type="SheetLabel" data-datafield="OccupationName" style="">职业类型</span>
                    </div>
                    <div id="control53" class="col-md-4">
                        <input id="Control59" type="text" data-datafield="OccupationName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title54" class="col-md-2">
                        <span id="Label60" data-type="SheetLabel" data-datafield="SubOccupationName" style="">职业子类型</span>
                    </div>
                    <div id="control54" class="col-md-4">
                        <input id="Control60" type="text" data-datafield="SubOccupationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title55" class="col-md-2">
                        <span id="Label61" data-type="SheetLabel" data-datafield="DesginationName" style="">职位</span>
                    </div>
                    <div id="control55" class="col-md-4">
                        <input id="Control61" type="text" data-datafield="DesginationName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title56" class="col-md-2">
                        <span id="Label62" data-type="SheetLabel" data-datafield="JobGroupName" style="">工作组</span>
                    </div>
                    <div id="control56" class="col-md-4">
                        <input id="Control62" type="text" data-datafield="JobGroupName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title57" class="col-md-2">
                        <span id="Label63" data-type="SheetLabel" data-datafield="SalaryRangeName" style="">估计收入</span>
                    </div>
                    <div id="control57" class="col-md-4">
                        <input id="Control63" type="text" data-datafield="SalaryRangeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title58" class="col-md-2">
                        <span id="Label64" data-type="SheetLabel" data-datafield="Consent" style="">同意</span>
                    </div>
                    <div id="control58" class="col-md-4">
                        <input id="Control64" type="text" data-datafield="Consent" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title59" class="col-md-2">
                        <span id="Label65" data-type="SheetLabel" data-datafield="VIPInd" style="">贵宾</span>
                    </div>
                    <div id="control59" class="col-md-4">
                        <input id="Control65" type="text" data-datafield="VIPInd" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title60" class="col-md-2">
                        <span id="Label66" data-type="SheetLabel" data-datafield="StaffInd" style="">工作人员</span>
                    </div>
                    <div id="control60" class="col-md-4">
                        <input id="Control66" type="text" data-datafield="StaffInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title61" class="col-md-2 light">
                        <span id="Label67" data-type="SheetLabel" data-datafield="BlackListNoRecordInd" style="">黑名单无记录</span>
                    </div>
                    <div id="control61" class="col-md-4 signcolor">
                        <input id="Control67" type="text" data-datafield="BlackListNoRecordInd" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title62" class="col-md-2 light">
                        <span id="Label68" data-type="SheetLabel" data-datafield="BlacklistInd" style="">外部信用记录</span>
                    </div>
                    <div id="control62" class="col-md-4 signcolor">
                        <input id="Control68" type="text" data-datafield="BlacklistInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title63" class="col-md-2 light">
                        <span id="Label69" data-type="SheetLabel" data-datafield="lienee" style="">抵押人</span>
                    </div>
                    <div id="control63" class="col-md-4 signcolor">
                        <input id="Control69" type="text" data-datafield="lienee" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title64" class="col-md-2 light">
                        <span id="Label70" data-type="SheetLabel" data-datafield="ActualSalary" style="">月收入</span>
                    </div>
                    <div id="control64" class="col-md-4 signcolor">
                        <input id="Control70" type="text" data-datafield="ActualSalary" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
            <div id="div106">
                <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divgzxx',this)">
                    <label id="Label9" data-en_us="Sheet information">工作信息</label>
                </div>
                <div id="divgzxx" style="width: 100%;">
                    <div class="row tableContent">
                        <div id="title65" class="col-md-2 light">
                            <span id="Label71" data-type="SheetLabel" data-datafield="ZjrCompanyName" style="">公司名称</span>
                        </div>
                        <div id="control65" class="col-md-10 signcolor">
                            <input id="Control71" type="text" data-datafield="ZjrCompanyName" data-type="SheetTextBox" style="" />
                        </div>
                    </div>
                    <div class="row">
                        <div id="title67" class="col-md-2 light">
                            <span id="Label72" data-type="SheetLabel" data-datafield="BusinessTypeName" style="">企业性质</span>
                        </div>
                        <div id="control67" class="col-md-4 signcolor">
                            <input id="Control72" type="text" data-datafield="BusinessTypeName" data-type="SheetTextBox" style="">
                        </div>
                        <div id="title68" class="col-md-2">
                            <span id="Label73" data-type="SheetLabel" data-datafield="positionName" style="">职位</span>
                        </div>
                        <div id="control68" class="col-md-4">
                            <input id="Control73" type="text" data-datafield="positionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="row">
                        <div id="title69" class="col-md-2">
                            <span id="Label74" data-type="SheetLabel" data-datafield="comapnyProvinceName" style="">单位省份</span>
                        </div>
                        <div id="control69" class="col-md-4">
                            <input id="Control74" type="text" data-datafield="comapnyProvinceName" data-type="SheetTextBox" style="">
                        </div>
                        <div id="title70" class="col-md-2 light">
                            <span id="Label75" data-type="SheetLabel" data-datafield="companyCityName" style="">单位城市</span>
                        </div>
                        <div id="control70" class="col-md-4 signcolor">
                            <input id="Control75" type="text" data-datafield="companyCityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="row">
                        <div id="title71" class="col-md-2 light">
                            <span id="Label76" data-type="SheetLabel" data-datafield="phonenumber" style="">电话</span>
                        </div>
                        <div id="control71" class="col-md-4 signcolor">
                            <input id="Control76" type="text" data-datafield="phonenumber" data-type="SheetTextBox" style="">
                        </div>
                        <div id="title72" class="col-md-2">
                            <span id="Label77" data-type="SheetLabel" data-datafield="Fax" style="">传真</span>
                        </div>
                        <div id="control72" class="col-md-4">
                            <input id="Control77" type="text" data-datafield="Fax" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title73" class="col-md-2">
                            <span id="Label78" data-type="SheetLabel" data-datafield="companyAddress" style="">公司地址 </span>
                        </div>
                        <div id="control73" class="col-md-10">
                            <textarea id="Control78" data-datafield="companyAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="row">
                        <div id="title75" class="col-md-2">
                            <span id="Label79" data-type="SheetLabel" data-datafield="EmployerType" style="">雇主类型</span>
                        </div>
                        <div id="control75" class="col-md-4">
                            <input id="Control79" type="text" data-datafield="EmployerType" data-type="SheetTextBox" style="">
                        </div>
                        <div id="title76" class="col-md-2">
                            <span id="Label80" data-type="SheetLabel" data-datafield="NoOfEmployees" style="">雇员人数</span>
                        </div>
                        <div id="control76" class="col-md-4">
                            <input id="Control80" type="text" data-datafield="NoOfEmployees" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="row">
                        <div id="title77" class="col-md-2">
                            <span id="Label81" data-type="SheetLabel" data-datafield="JobDescription" style="">工作描述 </span>
                        </div>
                        <div id="control77" class="col-md-4">
                            <input id="Control81" type="text" data-datafield="JobDescription" data-type="SheetTextBox" style="">
                        </div>
                        <div id="title78" class="col-md-2 light">
                            <span id="Label82" data-type="SheetLabel" data-datafield="timeinyear" style="">工作年限（年）</span>
                        </div>
                        <div id="control78" class="col-md-4 signcolor">
                            <input id="Control82" type="text" data-datafield="timeinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="row">
                        <div id="title79" class="col-md-2">
                            <span id="Label83" data-type="SheetLabel" data-datafield="timeinmonth" style="">工作年限（月） </span>
                        </div>
                        <div id="control79" class="col-md-4">
                            <input id="Control83" type="text" data-datafield="timeinmonth" data-type="SheetTextBox" style="">
                        </div>
                        <div id="space80" class="col-md-2">
                        </div>
                        <div id="spaceControl80" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title81" class="col-md-2">
                            <span id="Label84" data-type="SheetLabel" data-datafield="companyComments" style="">公司评论 </span>
                        </div>
                        <div id="control81" class="col-md-10">
                            <textarea id="Control84" data-datafield="companyComments" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divsqrdz',this)">
                <label id="Label10" data-en_us="Sheet information">地址</label>
            </div>
            <div id="divsqrdz">
                <div class="row tableContent">
                    <div id="title83" class="col-md-2 light">
                        <span id="Label85" data-type="SheetLabel" data-datafield="currentLivingAddress" style="">申请人地址</span>
                    </div>
                    <div id="control83" class="col-md-10 signcolor">
                        <textarea id="Control85" data-datafield="currentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title85" class="col-md-2 light">
                        <span id="Label86" data-type="SheetLabel" data-datafield="AddressId" style="">户籍地址</span>
                    </div>
                    <div id="control85" class="col-md-10 signcolor">
                        <textarea id="Control86" data-datafield="AddressId" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row">
                    <div id="title87" class="col-md-2">
                        <span id="Label87" data-type="SheetLabel" data-datafield="defaultmailingaddress" style="">默认邮寄地址</span>
                    </div>
                    <div id="control87" class="col-md-4">
                        <input id="Control87" type="text" data-datafield="defaultmailingaddress" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title88" class="col-md-2">
                        <span id="Label88" data-type="SheetLabel" data-datafield="hukouaddress" style="">户籍地址</span>
                    </div>
                    <div id="control88" class="col-md-4">
                        <input id="Control88" type="text" data-datafield="hukouaddress" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title89" class="col-md-2">
                        <span id="Label89" data-type="SheetLabel" data-datafield="countryName" style="">国家</span>
                    </div>
                    <div id="control89" class="col-md-4">
                        <input id="Control89" type="text" data-datafield="countryName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title90" class="col-md-2 light">
                        <span id="Label90" data-type="SheetLabel" data-datafield="nativedistrict" style="">籍贯</span>
                    </div>
                    <div id="control90" class="col-md-4 signcolor">
                        <input id="Control90" type="text" data-datafield="nativedistrict" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title91" class="col-md-2">
                        <span id="Label91" data-type="SheetLabel" data-datafield="birthpalaceprovince" style="">出生地省市县（区）</span>
                    </div>
                    <div id="control91" class="col-md-4">
                        <input id="Control91" type="text" data-datafield="birthpalaceprovince" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title92" class="col-md-2 light">
                        <span id="Label92" data-type="SheetLabel" data-datafield="hukouprovinceName" style="">户籍省份</span>
                    </div>
                    <div id="control92" class="col-md-4 signcolor">
                        <input id="Control92" type="text" data-datafield="hukouprovinceName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title93" class="col-md-2">
                        <span id="Label93" data-type="SheetLabel" data-datafield="hukoucityName" style="">户籍城市</span>
                    </div>
                    <div id="control93" class="col-md-4">
                        <input id="Control93" type="text" data-datafield="hukoucityName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title94" class="col-md-2">
                        <span id="Label94" data-type="SheetLabel" data-datafield="companypostCode" style="">公司邮编</span>
                    </div>
                    <div id="control94" class="col-md-4">
                        <input id="Control94" type="text" data-datafield="companypostCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title95" class="col-md-2">
                        <span id="Label95" data-type="SheetLabel" data-datafield="postcode" style="">邮编</span>
                    </div>
                    <div id="control95" class="col-md-4">
                        <input id="Control95" type="text" data-datafield="postcode" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title96" class="col-md-2">
                        <span id="Label96" data-type="SheetLabel" data-datafield="addresstypeName" style="">地址类型</span>
                    </div>
                    <div id="control96" class="col-md-4">
                        <input id="Control96" type="text" data-datafield="addresstypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title97" class="col-md-2">
                        <span id="Label97" data-type="SheetLabel" data-datafield="addressstatusName" style="">地址状态</span>
                    </div>
                    <div id="control97" class="col-md-4">
                        <input id="Control97" type="text" data-datafield="addressstatusName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title98" class="col-md-2">
                        <span id="Label98" data-type="SheetLabel" data-datafield="propertytypeName" style="">房产类型</span>
                    </div>
                    <div id="control98" class="col-md-4">
                        <input id="Control98" type="text" data-datafield="propertytypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title99" class="col-md-2">
                        <span id="Label99" data-type="SheetLabel" data-datafield="residencetypeName" style="">住宅类型</span>
                    </div>
                    <div id="control99" class="col-md-4">
                        <input id="Control99" type="text" data-datafield="residencetypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title100" class="col-md-2">
                        <span id="Label100" data-type="SheetLabel" data-datafield="livingsince" style="">开始居住日期</span>
                    </div>
                    <div id="control100" class="col-md-4">
                        <input id="Control100" type="text" data-datafield="livingsince" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title101" class="col-md-2">
                        <span id="Label101" data-type="SheetLabel" data-datafield="homevalue" style="">房屋价值（万元)</span>
                    </div>
                    <div id="control101" class="col-md-4">
                        <input id="Control101" type="text" data-datafield="homevalue" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title102" class="col-md-2">
                        <span id="Label102" data-type="SheetLabel" data-datafield="stayinyear" style="">居住年限</span>
                    </div>
                    <div id="control102" class="col-md-4">
                        <input id="Control102" type="text" data-datafield="stayinyear" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title103" class="col-md-2">
                        <span id="Label103" data-type="SheetLabel" data-datafield="countrycodeName" style="">国家代码</span>
                    </div>
                    <div id="control103" class="col-md-4">
                        <input id="Control103" type="text" data-datafield="countrycodeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title104" class="col-md-2">
                        <span id="Label104" data-type="SheetLabel" data-datafield="areaCode" style="">地区代码</span>
                    </div>
                    <div id="control104" class="col-md-4">
                        <input id="Control104" type="text" data-datafield="areaCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title105" class="col-md-2 light">
                        <span id="Label105" data-type="SheetLabel" data-datafield="phoneNo" style="">电话</span>
                    </div>
                    <div id="control105" class="col-md-4 signcolor">
                        <input id="Control105" type="text" data-datafield="phoneNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title106" class="col-md-2">
                        <span id="Label106" data-type="SheetLabel" data-datafield="extension" style="">分机</span>
                    </div>
                    <div id="control106" class="col-md-4">
                        <input id="Control106" type="text" data-datafield="extension" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title107" class="col-md-2">
                        <span id="Label107" data-type="SheetLabel" data-datafield="phonetypeName" style="">电话类型</span>
                    </div>
                    <div id="control107" class="col-md-4">
                        <input id="Control107" type="text" data-datafield="phonetypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divlxr',this)">
                <label data-en_us="Sheet information">联系人</label>
            </div>
            <div id="divlxr">
                <div class="row">
                    <div id="title113" class="col-md-2">
                        <span id="Label113" data-type="SheetLabel" data-datafield="name1" style="">名称</span>
                    </div>
                    <div id="control113" class="col-md-4">
                        <input id="Control113" type="text" data-datafield="name1" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title114" class="col-md-2">
                        <span id="Label114" data-type="SheetLabel" data-datafield="relationshipName" style="">关系</span>
                    </div>
                    <div id="control114" class="col-md-4">
                        <input id="Control114" type="text" data-datafield="relationshipName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title115" class="col-md-2">
                        <span id="Label115" data-type="SheetLabel" data-datafield="address" style="">联系人地址</span>
                    </div>
                    <div id="control115" class="col-md-4">
                        <input id="Control115" type="text" data-datafield="address" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title116" class="col-md-2">
                        <span id="Label116" data-type="SheetLabel" data-datafield="lxprovinceCode" style="">联系人省份</span>
                    </div>
                    <div id="control116" class="col-md-4">
                        <input id="Control116" type="text" data-datafield="lxprovinceCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title117" class="col-md-2">
                        <span id="Label117" data-type="SheetLabel" data-datafield="lxcityName" style="">联系人城市</span>
                    </div>
                    <div id="control117" class="col-md-4">
                        <input id="Control117" type="text" data-datafield="lxcityName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title118" class="col-md-2">
                        <span id="Label118" data-type="SheetLabel" data-datafield="lxpostCode" style="">联系人邮编</span>
                    </div>
                    <div id="control118" class="col-md-4">
                        <input id="Control118" type="text" data-datafield="lxpostCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title121" class="col-md-2">
                        <span id="Label120" data-type="SheetLabel" data-datafield="lxphoneNo" style="">联系人电话</span>
                    </div>
                    <div id="control121" class="col-md-4">
                        <input id="Control120" type="text" data-datafield="lxphoneNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title122" class="col-md-2">
                        <span id="Label121" data-type="SheetLabel" data-datafield="lxmobile" style="">联系人手机</span>
                    </div>
                    <div id="control122" class="col-md-4">
                        <input id="Control121" type="text" data-datafield="lxmobile" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title119" class="col-md-2">
                        <span id="Label119" data-type="SheetLabel" data-datafield="lxhokouName" style="">联系人户口所在地</span>
                    </div>
                    <div id="control119" class="col-md-10">
                        <textarea id="Control119" data-datafield="lxhokouName" data-type="SheetTextBox" style="">					</textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divGJR',this)">
            <label id="Label6" data-en_us="Sheet information">共借人信息</label>
        </div>
        <div class="divContent" id="divGJR">
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInfo1',this)">
                <label id="Label811" data-en_us="Sheet information">基本信息</label>
            </div>
            <div id="divBascSQRInfo1">
                <div class="row">
                    <div id="title123" class="col-md-2">
                        <span id="Label122" data-type="SheetLabel" data-datafield="GjThaiFirstName" style="">共借人姓名（中文）</span>
                    </div>
                    <div id="control123" class="col-md-4">
                        <input id="Control122" type="text" data-datafield="GjThaiFirstName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title124" class="col-md-2">
                        <span id="Label123" data-type="SheetLabel" data-datafield="GjIdCarTypeName" style="">共借人证件类型</span>
                    </div>
                    <div id="control124" class="col-md-4">
                        <input id="Control123" type="text" data-datafield="GjIdCarTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title125" class="col-md-2">
                        <span id="Label124" data-type="SheetLabel" data-datafield="GjIdCardNo" style="">共借人证件号码</span>
                    </div>
                    <div id="control125" class="col-md-4">
                        <input id="Control124" type="text" data-datafield="GjIdCardNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title126" class="col-md-2">
                        <span id="Label125" data-type="SheetLabel" data-datafield="GjMaritalStatusName" style="">共借人婚姻状况</span>
                    </div>
                    <div id="control126" class="col-md-4">
                        <input id="Control125" type="text" data-datafield="GjMaritalStatusName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title127" class="col-md-2">
                        <span id="Label126" data-type="SheetLabel" data-datafield="GjgenderName" style="">共借人性别</span>
                    </div>
                    <div id="control127" class="col-md-4">
                        <input id="Control126" type="text" data-datafield="GjgenderName" data-type="SheetTextBox" style="">
                    </div>
                    <%--<div id="space128" class="col-md-2">
                    </div>
                    <div id="spaceControl128" class="col-md-4">
                    </div>--%>
                    <div id="title129" class="col-md-2">
                        <span id="Label127" data-type="SheetLabel" data-datafield="GjHokouName" style="">共借人户口所在地</span>
                    </div>
                    <div id="control129" class="col-md-4">
                        <textarea id="Control127" data-datafield="GjHokouName" data-type="SheetTextBox" style="">					</textarea>
                    </div>
                </div>
                <%--<div class="row">
                    
                </div>--%>
                <div class="row">
                    <div id="title131" class="col-md-2">
                        <span id="Label128" data-type="SheetLabel" data-datafield="GjDateOfBirth" style="">共借人出生日期</span>
                    </div>
                    <div id="control131" class="col-md-4">
                        <input id="Control128" type="text" data-datafield="GjDateOfBirth" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title132" class="col-md-2">
                        <span id="Label129" data-type="SheetLabel" data-datafield="GjCitizenshipName" style="">共借人国籍</span>
                    </div>
                    <div id="control132" class="col-md-4">
                        <input id="Control129" type="text" data-datafield="GjCitizenshipName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title133" class="col-md-2">
                        <span id="Label130" data-type="SheetLabel" data-datafield="GjIdCardIssueDate" style="">共借人证件发行日</span>
                    </div>
                    <div id="control133" class="col-md-4">
                        <input id="Control130" type="text" data-datafield="GjIdCardIssueDate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title134" class="col-md-2">
                        <span id="Label131" data-type="SheetLabel" data-datafield="GjIdCardExpiryDate" style="">共借人证件到期日</span>
                    </div>
                    <div id="control134" class="col-md-4">
                        <input id="Control131" type="text" data-datafield="GjIdCardExpiryDate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title135" class="col-md-2">
                        <span id="Label132" data-type="SheetLabel" data-datafield="GjLicenseNo" style="">共借人驾照号码</span>
                    </div>
                    <div id="control135" class="col-md-4">
                        <input id="Control132" type="text" data-datafield="GjLicenseNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title136" class="col-md-2">
                        <span id="Label133" data-type="SheetLabel" data-datafield="GjLicenseExpiryDate" style="">共借人驾照到期日</span>
                    </div>
                    <div id="control136" class="col-md-4">
                        <input id="Control133" type="text" data-datafield="GjLicenseExpiryDate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title137" class="col-md-2">
                        <span id="Label134" data-type="SheetLabel" data-datafield="GjThaiTitleName" style="">共借人头衔</span>
                    </div>
                    <div id="control137" class="col-md-4">
                        <input id="Control134" type="text" data-datafield="GjThaiTitleName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title138" class="col-md-2">
                        <span id="Label135" data-type="SheetLabel" data-datafield="GjTitleName" style="">共借人头衔（英文）</span>
                    </div>
                    <div id="control138" class="col-md-4">
                        <input id="Control135" type="text" data-datafield="GjTitleName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title139" class="col-md-2">
                        <span id="Label136" data-type="SheetLabel" data-datafield="GjDrivingLicenseStatusName" style="">共借人驾照状态</span>
                    </div>
                    <div id="control139" class="col-md-4">
                        <input id="Control136" type="text" data-datafield="GjDrivingLicenseStatusName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title140" class="col-md-2">
                        <span id="Label137" data-type="SheetLabel" data-datafield="GjEngFirstName" style="">共借人名（英文）</span>
                    </div>
                    <div id="control140" class="col-md-4">
                        <input id="Control137" type="text" data-datafield="GjEngFirstName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title141" class="col-md-2">
                        <span id="Label138" data-type="SheetLabel" data-datafield="GjEngLastName" style="">共借人姓（英文）</span>
                    </div>
                    <div id="control141" class="col-md-4">
                        <input id="Control138" type="text" data-datafield="GjEngLastName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title142" class="col-md-2">
                        <span id="Label139" data-type="SheetLabel" data-datafield="GjEngMiddleName" style="">共借人中间名字</span>
                    </div>
                    <div id="control142" class="col-md-4">
                        <input id="Control139" type="text" data-datafield="GjEngMiddleName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title143" class="col-md-2">
                        <span id="Label140" data-type="SheetLabel" data-datafield="GjFormerName" style="">共借人曾用名</span>
                    </div>
                    <div id="control143" class="col-md-4">
                        <input id="Control140" type="text" data-datafield="GjFormerName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title144" class="col-md-2">
                        <span id="Label141" data-type="SheetLabel" data-datafield="GjNationName" style="">共借人民族</span>
                    </div>
                    <div id="control144" class="col-md-4">
                        <input id="Control141" type="text" data-datafield="GjNationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title145" class="col-md-2">
                        <span id="Label142" data-type="SheetLabel" data-datafield="GjIssuingAuthority" style="">共借人签发机关</span>
                    </div>
                    <div id="control145" class="col-md-4">
                        <input id="Control142" type="text" data-datafield="GjIssuingAuthority" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title146" class="col-md-2">
                        <span id="Label143" data-type="SheetLabel" data-datafield="GjNumberOfDependents" style="">共借人供养人数</span>
                    </div>
                    <div id="control146" class="col-md-4">
                        <input id="Control143" type="text" data-datafield="GjNumberOfDependents" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title147" class="col-md-2">
                        <span id="Label144" data-type="SheetLabel" data-datafield="GjHouseOwnerName" style="">共借人房产所有人</span>
                    </div>
                    <div id="control147" class="col-md-4">
                        <input id="Control144" type="text" data-datafield="GjHouseOwnerName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title148" class="col-md-2">
                        <span id="Label145" data-type="SheetLabel" data-datafield="GjNoOfFamilyMembers" style="">共借人家庭人数</span>
                    </div>
                    <div id="control148" class="col-md-4">
                        <input id="Control145" type="text" data-datafield="GjNoOfFamilyMembers" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title149" class="col-md-2">
                        <span id="Label146" data-type="SheetLabel" data-datafield="GjChildrenFlag" style="">共借人是否是子女</span>
                    </div>
                    <div id="control149" class="col-md-4">
                        <input id="Control146" type="text" data-datafield="GjChildrenFlag" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title150" class="col-md-2">
                        <span id="Label147" data-type="SheetLabel" data-datafield="GjEmploymentTypeName" style="">共借人雇员类型</span>
                    </div>
                    <div id="control150" class="col-md-4">
                        <input id="Control147" type="text" data-datafield="GjEmploymentTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title151" class="col-md-2">
                        <span id="Label148" data-type="SheetLabel" data-datafield="GjEmailAddress" style="">共借人邮箱地址</span>
                    </div>
                    <div id="control151" class="col-md-4">
                        <input id="Control148" type="text" data-datafield="GjEmailAddress" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title152" class="col-md-2">
                        <span id="Label149" data-type="SheetLabel" data-datafield="GjEducationName" style="">共借人教育程度</span>
                    </div>
                    <div id="control152" class="col-md-4">
                        <input id="Control149" type="text" data-datafield="GjEducationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title153" class="col-md-2">
                        <span id="Label150" data-type="SheetLabel" data-datafield="GjIndustryTypeName" style="">共借人行业类型</span>
                    </div>
                    <div id="control153" class="col-md-4">
                        <input id="Control150" type="text" data-datafield="GjIndustryTypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title154" class="col-md-2">
                        <span id="Label151" data-type="SheetLabel" data-datafield="GjIndustrySubTypeName" style="">共借人行业子类型</span>
                    </div>
                    <div id="control154" class="col-md-4">
                        <input id="Control151" type="text" data-datafield="GjIndustrySubTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title155" class="col-md-2">
                        <span id="Label152" data-type="SheetLabel" data-datafield="GjOccupationName" style="">共借人职业类型</span>
                    </div>
                    <div id="control155" class="col-md-4">
                        <input id="Control152" type="text" data-datafield="GjOccupationName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title156" class="col-md-2">
                        <span id="Label153" data-type="SheetLabel" data-datafield="GjSubOccupationName" style="">共借人职业子类型</span>
                    </div>
                    <div id="control156" class="col-md-4">
                        <input id="Control153" type="text" data-datafield="GjSubOccupationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title157" class="col-md-2">
                        <span id="Label154" data-type="SheetLabel" data-datafield="GjDesginationName" style="">共借人职位</span>
                    </div>
                    <div id="control157" class="col-md-4">
                        <input id="Control154" type="text" data-datafield="GjDesginationName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title158" class="col-md-2">
                        <span id="Label155" data-type="SheetLabel" data-datafield="GjJobGroupName" style="">共借人工作组</span>
                    </div>
                    <div id="control158" class="col-md-4">
                        <input id="Control155" type="text" data-datafield="GjJobGroupName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title159" class="col-md-2">
                        <span id="Label156" data-type="SheetLabel" data-datafield="GjSalaryRangeName" style="">共借人估计收入</span>
                    </div>
                    <div id="control159" class="col-md-4">
                        <input id="Control156" type="text" data-datafield="GjSalaryRangeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title160" class="col-md-2">
                        <span id="Label157" data-type="SheetLabel" data-datafield="GjConsent" style="">共借人同意</span>
                    </div>
                    <div id="control160" class="col-md-4">
                        <input id="Control157" type="text" data-datafield="GjConsent" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title161" class="col-md-2">
                        <span id="Label158" data-type="SheetLabel" data-datafield="GjVIPInd" style="">共借人贵宾</span>
                    </div>
                    <div id="control161" class="col-md-4">
                        <input id="Control158" type="text" data-datafield="GjVIPInd" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title162" class="col-md-2">
                        <span id="Label159" data-type="SheetLabel" data-datafield="GjStaffInd" style="">共借人工作人员</span>
                    </div>
                    <div id="control162" class="col-md-4">
                        <input id="Control159" type="text" data-datafield="GjStaffInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title163" class="col-md-2">
                        <span id="Label160" data-type="SheetLabel" data-datafield="GjBlackListNoRecordInd" style="">共借人黑名单无记录</span>
                    </div>
                    <div id="control163" class="col-md-4">
                        <input id="Control160" type="text" data-datafield="GjBlackListNoRecordInd" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title164" class="col-md-2">
                        <span id="Label161" data-type="SheetLabel" data-datafield="GjBlacklistInd" style="">共借人外部信用记录</span>
                    </div>
                    <div id="control164" class="col-md-4">
                        <input id="Control161" type="text" data-datafield="GjBlacklistInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title165" class="col-md-2">
                        <span id="Label162" data-type="SheetLabel" data-datafield="Gjlienee" style="">共借人抵押人</span>
                    </div>
                    <div id="control165" class="col-md-4">
                        <input id="Control162" type="text" data-datafield="Gjlienee" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title166" class="col-md-2">
                        <span id="Label163" data-type="SheetLabel" data-datafield="GjActualSalary" style="">共借人月收入</span>
                    </div>
                    <div id="control166" class="col-md-4">
                        <input id="Control163" type="text" data-datafield="GjActualSalary" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title167" class="col-md-2">
                        <span id="Label164" data-type="SheetLabel" data-datafield="Gjspouse" style="">共借人是否隐藏配偶</span>
                    </div>
                    <div id="control167" class="col-md-4">
                        <input id="Control164" type="text" data-datafield="Gjspouse" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title168" class="col-md-2">
                        <span id="Label165" data-type="SheetLabel" data-datafield="GjrelationShipName" style="">共借人和主借人的关系</span>
                    </div>
                    <div id="control168" class="col-md-4">
                        <input id="Control165" type="text" data-datafield="GjrelationShipName" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInfo2',this)">
                <label data-en_us="Sheet information">工作信息</label>
            </div>
            <div id="divBascSQRInfo2">
                <div class="row">
                    <div id="title169" class="col-md-2">
                        <span id="Label166" data-type="SheetLabel" data-datafield="GjcompanyName" style="">共借人公司名称</span>
                    </div>
                    <div id="control169" class="col-md-4">
                        <input id="Control166" type="text" data-datafield="GjcompanyName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title170" class="col-md-2">
                        <span id="Label167" data-type="SheetLabel" data-datafield="GjBusinessTypeName" style="">共借人企业性质</span>
                    </div>
                    <div id="control170" class="col-md-4">
                        <input id="Control167" type="text" data-datafield="GjBusinessTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title171" class="col-md-2">
                        <span id="Label168" data-type="SheetLabel" data-datafield="GjpositionName" style="">共借人职位</span>
                    </div>
                    <div id="control171" class="col-md-4">
                        <input id="Control168" type="text" data-datafield="GjpositionName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title172" class="col-md-2">
                        <span id="Label169" data-type="SheetLabel" data-datafield="GjProvinceName" style="">共借人公司省份</span>
                    </div>
                    <div id="control172" class="col-md-4">
                        <input id="Control169" type="text" data-datafield="GjProvinceName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title173" class="col-md-2">
                        <span id="Label170" data-type="SheetLabel" data-datafield="GjCityName" style="">共借人公司城市</span>
                    </div>
                    <div id="control173" class="col-md-4">
                        <input id="Control170" type="text" data-datafield="GjCityName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title174" class="col-md-2">
                        <span id="Label171" data-type="SheetLabel" data-datafield="Gjphonenumber" style="">共借人公司电话 </span>
                    </div>
                    <div id="control174" class="col-md-4">
                        <input id="Control171" type="text" data-datafield="Gjphonenumber" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title175" class="col-md-2">
                        <span id="Label172" data-type="SheetLabel" data-datafield="GjFax" style="">共借人公司传真</span>
                    </div>
                    <div id="control175" class="col-md-4">
                        <input id="Control172" type="text" data-datafield="GjFax" data-type="SheetTextBox" style="">
                    </div>
                    <div id="space176" class="col-md-2">
                    </div>
                    <div id="spaceControl176" class="col-md-4">
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title177" class="col-md-2">
                        <span id="Label173" data-type="SheetLabel" data-datafield="GjcompanyAddress" style="">共借人公司地址</span>
                    </div>
                    <div id="control177" class="col-md-10">
                        <textarea id="Control173" data-datafield="GjcompanyAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row">
                    <div id="title179" class="col-md-2">
                        <span id="Label174" data-type="SheetLabel" data-datafield="GjcompanypostCode" style="">共借人公司邮编</span>
                    </div>
                    <div id="control179" class="col-md-4">
                        <input id="Control174" type="text" data-datafield="GjcompanypostCode" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title180" class="col-md-2">
                        <span id="Label175" data-type="SheetLabel" data-datafield="GjEmployerType" style="">共借人雇主类型</span>
                    </div>
                    <div id="control180" class="col-md-4">
                        <input id="Control175" type="text" data-datafield="GjEmployerType" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title181" class="col-md-2">
                        <span id="Label176" data-type="SheetLabel" data-datafield="GjNoOfEmployees" style="">共借人雇员人数</span>
                    </div>
                    <div id="control181" class="col-md-4">
                        <input id="Control176" type="text" data-datafield="GjNoOfEmployees" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title182" class="col-md-2">
                        <span id="Label177" data-type="SheetLabel" data-datafield="GjJobDescription" style="">共借人工作描述</span>
                    </div>
                    <div id="control182" class="col-md-4">
                        <input id="Control177" type="text" data-datafield="GjJobDescription" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title183" class="col-md-2">
                        <span id="Label178" data-type="SheetLabel" data-datafield="Gjtimeinyear" style="">共借人工作年限（年）</span>
                    </div>
                    <div id="control183" class="col-md-4">
                        <input id="Control178" type="text" data-datafield="Gjtimeinyear" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title184" class="col-md-2">
                        <span id="Label179" data-type="SheetLabel" data-datafield="Gjtimeinmonth" style="">共借人工作年限（月）</span>
                    </div>
                    <div id="control184" class="col-md-4">
                        <input id="Control179" type="text" data-datafield="Gjtimeinmonth" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title185" class="col-md-2">
                        <span id="Label180" data-type="SheetLabel" data-datafield="GjcompanyComments" style="">共借人公司评论</span>
                    </div>
                    <div id="control185" class="col-md-10">
                        <textarea id="Control180" data-datafield="GjcompanyComments" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInf3',this)">
                <label data-en_us="Sheet information">地址</label>
            </div>
            <div id="divBascSQRInf3">
                <div class="row tableContent">
                    <div id="title187" class="col-md-2">
                        <span id="Label181" data-type="SheetLabel" data-datafield="GjcurrentLivingAddress" style="">共借人地址</span>
                    </div>
                    <div id="control187" class="col-md-10">
                        <textarea id="Control181" data-datafield="GjcurrentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title189" class="col-md-2">
                        <span id="Label182" data-type="SheetLabel" data-datafield="GjAddressId" style="">共借人户籍地址</span>
                    </div>
                    <div id="control189" class="col-md-10">
                        <textarea id="Control182" data-datafield="GjAddressId" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row">
                    <div id="title191" class="col-md-2">
                        <span id="Label183" data-type="SheetLabel" data-datafield="Gjdefaultmailingaddress" style="">共借人默认邮寄地址</span>
                    </div>
                    <div id="control191" class="col-md-4">
                        <input id="Control183" type="text" data-datafield="Gjdefaultmailingaddress" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title192" class="col-md-2">
                        <span id="Label184" data-type="SheetLabel" data-datafield="Gjhukouaddress" style="">共借人户籍地址</span>
                    </div>
                    <div id="control192" class="col-md-4">
                        <input id="Control184" type="text" data-datafield="Gjhukouaddress" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title193" class="col-md-2">
                        <span id="Label185" data-type="SheetLabel" data-datafield="GjcountryName" style="">共借人国家</span>
                    </div>
                    <div id="control193" class="col-md-4">
                        <input id="Control185" type="text" data-datafield="GjcountryName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title194" class="col-md-2">
                        <span id="Label186" data-type="SheetLabel" data-datafield="Gjnativedistrict" style="">共借人籍贯</span>
                    </div>
                    <div id="control194" class="col-md-4">
                        <input id="Control186" type="text" data-datafield="Gjnativedistrict" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title195" class="col-md-2">
                        <span id="Label187" data-type="SheetLabel" data-datafield="Gjbirthpalaceprovince" style="">共借人出生地省市县（区）</span>
                    </div>
                    <div id="control195" class="col-md-4">
                        <input id="Control187" type="text" data-datafield="Gjbirthpalaceprovince" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title196" class="col-md-2">
                        <span id="Label188" data-type="SheetLabel" data-datafield="GjhukouprovinceName" style="">共借人省份</span>
                    </div>
                    <div id="control196" class="col-md-4">
                        <input id="Control188" type="text" data-datafield="GjhukouprovinceName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title197" class="col-md-2">
                        <span id="Label189" data-type="SheetLabel" data-datafield="GjhukoucityName" style="">共借人城市</span>
                    </div>
                    <div id="control197" class="col-md-4">
                        <input id="Control189" type="text" data-datafield="GjhukoucityName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title198" class="col-md-2">
                        <span id="Label190" data-type="SheetLabel" data-datafield="Gjpostcode" style="">共借人邮编</span>
                    </div>
                    <div id="control198" class="col-md-4">
                        <input id="Control190" type="text" data-datafield="Gjpostcode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title199" class="col-md-2">
                        <span id="Label191" data-type="SheetLabel" data-datafield="GjaddresstypeName" style="">共借人地址类型</span>
                    </div>
                    <div id="control199" class="col-md-4">
                        <input id="Control191" type="text" data-datafield="GjaddresstypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title200" class="col-md-2">
                        <span id="Label192" data-type="SheetLabel" data-datafield="GjaddressstatusName" style="">共借人地址状态</span>
                    </div>
                    <div id="control200" class="col-md-4">
                        <input id="Control192" type="text" data-datafield="GjaddressstatusName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title201" class="col-md-2">
                        <span id="Label193" data-type="SheetLabel" data-datafield="GjpropertytypeName" style="">共借人房产类型</span>
                    </div>
                    <div id="control201" class="col-md-4">
                        <input id="Control193" type="text" data-datafield="GjpropertytypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title202" class="col-md-2">
                        <span id="Label194" data-type="SheetLabel" data-datafield="GjresidencetypeName" style="">共借人住宅类型</span>
                    </div>
                    <div id="control202" class="col-md-4">
                        <input id="Control194" type="text" data-datafield="GjresidencetypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title203" class="col-md-2">
                        <span id="Label195" data-type="SheetLabel" data-datafield="Gjlivingsince" style="">共借人开始居住日期</span>
                    </div>
                    <div id="control203" class="col-md-4">
                        <input id="Control195" type="text" data-datafield="Gjlivingsince" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title204" class="col-md-2">
                        <span id="Label196" data-type="SheetLabel" data-datafield="Gjhomevalue" style="">共借人房屋价值（万元)</span>
                    </div>
                    <div id="control204" class="col-md-4">
                        <input id="Control196" type="text" data-datafield="Gjhomevalue" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title205" class="col-md-2">
                        <span id="Label197" data-type="SheetLabel" data-datafield="Gjstayinyear" style="">共借人居住年限</span>
                    </div>
                    <div id="control205" class="col-md-4">
                        <input id="Control197" type="text" data-datafield="Gjstayinyear" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title206" class="col-md-2">
                        <span id="Label198" data-type="SheetLabel" data-datafield="Gjstayinmonth" style="">共借人居住月份</span>
                    </div>
                    <div id="control206" class="col-md-4">
                        <input id="Control198" type="text" data-datafield="Gjstayinmonth" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title207" class="col-md-2">
                        <span id="Label199" data-type="SheetLabel" data-datafield="GjcountrycodeName" style="">共借人国家代码</span>
                    </div>
                    <div id="control207" class="col-md-4">
                        <input id="Control199" type="text" data-datafield="GjcountrycodeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title208" class="col-md-2">
                        <span id="Label200" data-type="SheetLabel" data-datafield="GjareaCode" style="">共借人地区代码</span>
                    </div>
                    <div id="control208" class="col-md-4">
                        <input id="Control200" type="text" data-datafield="GjareaCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title209" class="col-md-2">
                        <span id="Label201" data-type="SheetLabel" data-datafield="GjphoneNo" style="">共借人电话</span>
                    </div>
                    <div id="control209" class="col-md-4">
                        <input id="Control201" type="text" data-datafield="GjphoneNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title210" class="col-md-2">
                        <span id="Label202" data-type="SheetLabel" data-datafield="Gjextension" style="">共借人分机</span>
                    </div>
                    <div id="control210" class="col-md-4">
                        <input id="Control202" type="text" data-datafield="Gjextension" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title211" class="col-md-2">
                        <span id="Label203" data-type="SheetLabel" data-datafield="GjphonetypeName" style="">共借人电话类型</span>
                    </div>
                    <div id="control211" class="col-md-4">
                        <input id="Control203" type="text" data-datafield="GjphonetypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
            <%--<div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRIno4',this)">
                <label data-en_us="Sheet information">共借人资产/负债</label>
            </div>
            <div id="divBascSQRIno4">
                <div class="row">
                    <div id="title212" class="col-md-2">
                        <span id="Label204" data-type="SheetLabel" data-datafield="GjDescritption" style="">共借人收入</span>
                    </div>
                    <div id="control212" class="col-md-4">
                        <input id="Control204" type="text" data-datafield="GjDescritption" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title213" class="col-md-2">
                        <span id="Label205" data-type="SheetLabel" data-datafield="GjDescritption1" style="">共借人支出</span>
                    </div>
                    <div id="control213" class="col-md-4">
                        <input id="Control205" type="text" data-datafield="GjDescritption1" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title214" class="col-md-2">
                        <span id="Label206" data-type="SheetLabel" data-datafield="GjValue" style="">共借人金额</span>
                    </div>
                    <div id="control214" class="col-md-4">
                        <input id="Control206" type="text" data-datafield="GjValue" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title215" class="col-md-2">
                        <span id="Label207" data-type="SheetLabel" data-datafield="GjFinancialratiodes" style="">共借人财务比率</span>
                    </div>
                    <div id="control215" class="col-md-4">
                        <input id="Control207" type="text" data-datafield="GjFinancialratiodes" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title216" class="col-md-2">
                        <span id="Label208" data-type="SheetLabel" data-datafield="GjEvalresult" style="">共借人财务比率金额</span>
                    </div>
                    <div id="control216" class="col-md-4">
                        <input id="Control208" type="text" data-datafield="GjEvalresult" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>--%>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divDBR',this)">
            <label id="Label7" data-en_us="Sheet information">担保人信息</label>
        </div>
        <div class="divContent" id="divDBR">
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRIno6',this)">
                <label data-en_us="Sheet information">担保人基本信息</label>
            </div>
            <div id="divBascSQRIno6">
                <div class="row">
                    <div id="title217" class="col-md-2">
                        <span id="Label209" data-type="SheetLabel" data-datafield="DbThaiFirstName" style="">担保人姓名（中文）</span>
                    </div>
                    <div id="control217" class="col-md-4">
                        <input id="Control209" type="text" data-datafield="DbThaiFirstName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title218" class="col-md-2">
                        <span id="Label210" data-type="SheetLabel" data-datafield="DbIdCarTypeName" style="">担保人证件类型</span>
                    </div>
                    <div id="control218" class="col-md-4">
                        <input id="Control210" type="text" data-datafield="DbIdCarTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title219" class="col-md-2">
                        <span id="Label211" data-type="SheetLabel" data-datafield="DbIdCardNo" style="">担保人证件号码</span>
                    </div>
                    <div id="control219" class="col-md-4">
                        <input id="Control211" type="text" data-datafield="DbIdCardNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title220" class="col-md-2">
                        <span id="Label212" data-type="SheetLabel" data-datafield="DbMaritalStatusName" style="">担保人婚姻状况</span>
                    </div>
                    <div id="control220" class="col-md-4">
                        <input id="Control212" type="text" data-datafield="DbMaritalStatusName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title221" class="col-md-2">
                        <span id="Label213" data-type="SheetLabel" data-datafield="DbgenderName" style="">担保人性别</span>
                    </div>
                    <div id="control221" class="col-md-4">
                        <input id="Control213" type="text" data-datafield="DbgenderName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title223" class="col-md-2">
                        <span id="Label214" data-type="SheetLabel" data-datafield="DbHokouName" style="">担保人户口所在地</span>
                    </div>
                    <div id="control223" class="col-md-4">
                        <textarea id="Control214" data-datafield="DbHokouName" data-type="SheetTextBox" style="">					</textarea>
                    </div>
                    <%--<div id="space222" class="col-md-2">
                    </div>
                    <div id="spaceControl222" class="col-md-4">
                    </div>--%>
                </div>
                <%--<div class="row">
                    
                </div>--%>
                <div class="row">
                    <div id="title225" class="col-md-2">
                        <span id="Label215" data-type="SheetLabel" data-datafield="DbDateOfBirth" style="">担保人出生日期</span>
                    </div>
                    <div id="control225" class="col-md-4">
                        <input id="Control215" type="text" data-datafield="DbDateOfBirth" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title226" class="col-md-2">
                        <span id="Label216" data-type="SheetLabel" data-datafield="DbCitizenshipName" style="">担保人国籍</span>
                    </div>
                    <div id="control226" class="col-md-4">
                        <input id="Control216" type="text" data-datafield="DbCitizenshipName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title227" class="col-md-2">
                        <span id="Label217" data-type="SheetLabel" data-datafield="DbIdCardIssueDate" style="">担保人证件发行日</span>
                    </div>
                    <div id="control227" class="col-md-4">
                        <input id="Control217" type="text" data-datafield="DbIdCardIssueDate" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title228" class="col-md-2">
                        <span id="Label218" data-type="SheetLabel" data-datafield="DbIdCardExpiryDate" style="">担保人证件到期日</span>
                    </div>
                    <div id="control228" class="col-md-4">
                        <input id="Control218" type="text" data-datafield="DbIdCardExpiryDate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title229" class="col-md-2">
                        <span id="Label219" data-type="SheetLabel" data-datafield="DbLicenseNo" style="">担保人驾照号码</span>
                    </div>
                    <div id="control229" class="col-md-4">
                        <input id="Control219" type="text" data-datafield="DbLicenseNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title230" class="col-md-2">
                        <span id="Label220" data-type="SheetLabel" data-datafield="DbLicenseExpiryDate" style="">担保人驾照到期日</span>
                    </div>
                    <div id="control230" class="col-md-4">
                        <input id="Control220" type="text" data-datafield="DbLicenseExpiryDate" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title231" class="col-md-2">
                        <span id="Label221" data-type="SheetLabel" data-datafield="DbThaiTitleName" style="">担保人头衔</span>
                    </div>
                    <div id="control231" class="col-md-4">
                        <input id="Control221" type="text" data-datafield="DbThaiTitleName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title232" class="col-md-2">
                        <span id="Label222" data-type="SheetLabel" data-datafield="DbTitleName" style="">担保人头衔（英文）</span>
                    </div>
                    <div id="control232" class="col-md-4">
                        <input id="Control222" type="text" data-datafield="DbTitleName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title233" class="col-md-2">
                        <span id="Label223" data-type="SheetLabel" data-datafield="DbDrivingLicenseStatusName" style="">担保人驾照状态</span>
                    </div>
                    <div id="control233" class="col-md-4">
                        <input id="Control223" type="text" data-datafield="DbDrivingLicenseStatusName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title234" class="col-md-2">
                        <span id="Label224" data-type="SheetLabel" data-datafield="DbEngFirstName" style="">担保人名（英文）</span>
                    </div>
                    <div id="control234" class="col-md-4">
                        <input id="Control224" type="text" data-datafield="DbEngFirstName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title235" class="col-md-2">
                        <span id="Label225" data-type="SheetLabel" data-datafield="DbEngLastName" style="">担保人中间名字</span>
                    </div>
                    <div id="control235" class="col-md-4">
                        <input id="Control225" type="text" data-datafield="DbEngLastName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title236" class="col-md-2">
                        <span id="Label226" data-type="SheetLabel" data-datafield="DbFormerName" style="">担保人曾用名</span>
                    </div>
                    <div id="control236" class="col-md-4">
                        <input id="Control226" type="text" data-datafield="DbFormerName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title237" class="col-md-2">
                        <span id="Label227" data-type="SheetLabel" data-datafield="DbNationCode" style="">担保人民族</span>
                    </div>
                    <div id="control237" class="col-md-4">
                        <input id="Control227" type="text" data-datafield="DbNationCode" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title238" class="col-md-2">
                        <span id="Label228" data-type="SheetLabel" data-datafield="DbIssuingAuthority" style="">担保人签发机关</span>
                    </div>
                    <div id="control238" class="col-md-4">
                        <input id="Control228" type="text" data-datafield="DbIssuingAuthority" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title239" class="col-md-2">
                        <span id="Label229" data-type="SheetLabel" data-datafield="DbNumberOfDependents" style="">担保人供养人数</span>
                    </div>
                    <div id="control239" class="col-md-4">
                        <input id="Control229" type="text" data-datafield="DbNumberOfDependents" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title240" class="col-md-2">
                        <span id="Label230" data-type="SheetLabel" data-datafield="DbHouseOwnerName" style="">担保人房产所有人</span>
                    </div>
                    <div id="control240" class="col-md-4">
                        <input id="Control230" type="text" data-datafield="DbHouseOwnerName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title241" class="col-md-2">
                        <span id="Label231" data-type="SheetLabel" data-datafield="DbNoOfFamilyMembers" style="">担保人家庭人数</span>
                    </div>
                    <div id="control241" class="col-md-4">
                        <input id="Control231" type="text" data-datafield="DbNoOfFamilyMembers" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title242" class="col-md-2">
                        <span id="Label232" data-type="SheetLabel" data-datafield="DbChildrenFlag" style="">担保人是否是子女</span>
                    </div>
                    <div id="control242" class="col-md-4">
                        <input id="Control232" type="text" data-datafield="DbChildrenFlag" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title243" class="col-md-2">
                        <span id="Label233" data-type="SheetLabel" data-datafield="DbEmploymentTypeName" style="">担保人雇员类型</span>
                    </div>
                    <div id="control243" class="col-md-4">
                        <input id="Control233" type="text" data-datafield="DbEmploymentTypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title244" class="col-md-2">
                        <span id="Label234" data-type="SheetLabel" data-datafield="DbEmailAddress" style="">担保人邮箱地址</span>
                    </div>
                    <div id="control244" class="col-md-4">
                        <input id="Control234" type="text" data-datafield="DbEmailAddress" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title245" class="col-md-2">
                        <span id="Label235" data-type="SheetLabel" data-datafield="DbEducationName" style="">担保人 教育程度 </span>
                    </div>
                    <div id="control245" class="col-md-4">
                        <input id="Control235" type="text" data-datafield="DbEducationName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title246" class="col-md-2">
                        <span id="Label236" data-type="SheetLabel" data-datafield="DbIndustryTypeName" style="">担保人行业类型</span>
                    </div>
                    <div id="control246" class="col-md-4">
                        <input id="Control236" type="text" data-datafield="DbIndustryTypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title247" class="col-md-2">
                        <span id="Label237" data-type="SheetLabel" data-datafield="DbIndustrySubTypeName" style="">担保人行业子类型</span>
                    </div>
                    <div id="control247" class="col-md-4">
                        <input id="Control237" type="text" data-datafield="DbIndustrySubTypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title248" class="col-md-2">
                        <span id="Label238" data-type="SheetLabel" data-datafield="DbOccupationName" style="">担保人职业类型</span>
                    </div>
                    <div id="control248" class="col-md-4">
                        <input id="Control238" type="text" data-datafield="DbOccupationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title249" class="col-md-2">
                        <span id="Label239" data-type="SheetLabel" data-datafield="DbSubOccupationName" style="">担保人职业子类型</span>
                    </div>
                    <div id="control249" class="col-md-4">
                        <input id="Control239" type="text" data-datafield="DbSubOccupationName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title250" class="col-md-2">
                        <span id="Label240" data-type="SheetLabel" data-datafield="DbDesginationName" style="">担保人职位</span>
                    </div>
                    <div id="control250" class="col-md-4">
                        <input id="Control240" type="text" data-datafield="DbDesginationName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title251" class="col-md-2">
                        <span id="Label241" data-type="SheetLabel" data-datafield="DbJobGroupName" style="">担保人工作组</span>
                    </div>
                    <div id="control251" class="col-md-4">
                        <input id="Control241" type="text" data-datafield="DbJobGroupName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title252" class="col-md-2">
                        <span id="Label242" data-type="SheetLabel" data-datafield="DbSalaryRangeName" style="">担保人估计收入</span>
                    </div>
                    <div id="control252" class="col-md-4">
                        <input id="Control242" type="text" data-datafield="DbSalaryRangeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title253" class="col-md-2">
                        <span id="Label243" data-type="SheetLabel" data-datafield="DbConsent" style="">担保人同意</span>
                    </div>
                    <div id="control253" class="col-md-4">
                        <input id="Control243" type="text" data-datafield="DbConsent" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title254" class="col-md-2">
                        <span id="Label244" data-type="SheetLabel" data-datafield="DbVIPInd" style="">担保人贵宾</span>
                    </div>
                    <div id="control254" class="col-md-4">
                        <input id="Control244" type="text" data-datafield="DbVIPInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title255" class="col-md-2">
                        <span id="Label245" data-type="SheetLabel" data-datafield="DbStaffInd" style="">担保人工作人员</span>
                    </div>
                    <div id="control255" class="col-md-4">
                        <input id="Control245" type="text" data-datafield="DbStaffInd" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title256" class="col-md-2">
                        <span id="Label246" data-type="SheetLabel" data-datafield="DbBlackListNoRecordInd" style="">担保人黑名单无记录</span>
                    </div>
                    <div id="control256" class="col-md-4">
                        <input id="Control246" type="text" data-datafield="DbBlackListNoRecordInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title257" class="col-md-2">
                        <span id="Label247" data-type="SheetLabel" data-datafield="DbBlacklistInd" style="">担保人外部信用记录</span>
                    </div>
                    <div id="control257" class="col-md-4">
                        <input id="Control247" type="text" data-datafield="DbBlacklistInd" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title258" class="col-md-2">
                        <span id="Label248" data-type="SheetLabel" data-datafield="Dblienee" style="">担保人抵押人</span>
                    </div>
                    <div id="control258" class="col-md-4">
                        <input id="Control248" type="text" data-datafield="Dblienee" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title259" class="col-md-2">
                        <span id="Label249" data-type="SheetLabel" data-datafield="DbActualSalary" style="">担保人月收入</span>
                    </div>
                    <div id="control259" class="col-md-4">
                        <input id="Control249" type="text" data-datafield="DbActualSalary" data-type="SheetTextBox" style="">
                    </div>
                    <div id="space260" class="col-md-2">
                    </div>
                    <div id="spaceControl260" class="col-md-4">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRIno5',this)">
                <label data-en_us="Sheet information">担保人工作信息</label>
            </div>
            <div id="divBascSQRIno5">
                <div class="row tableContent">
                    <div id="title261" class="col-md-2">
                        <span id="Label250" data-type="SheetLabel" data-datafield="DbCompanyName" style="">担保人公司名称</span>
                    </div>
                    <div id="control261" class="col-md-10">
                        <input id="Control250" data-datafield="DbCompanyName" data-type="SheetTextBox" style=""/>
                    </div>
                </div>
                <div class="row">
                    <div id="title263" class="col-md-2">
                        <span id="Label251" data-type="SheetLabel" data-datafield="DbBusinessTypeName" style="">担保人企业性质</span>
                    </div>
                    <div id="control263" class="col-md-4">
                        <input id="Control251" type="text" data-datafield="DbBusinessTypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title264" class="col-md-2">
                        <span id="Label252" data-type="SheetLabel" data-datafield="DbpositionName" style="">担保人职位</span>
                    </div>
                    <div id="control264" class="col-md-4">
                        <input id="Control252" type="text" data-datafield="DbpositionName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title265" class="col-md-2">
                        <span id="Label253" data-type="SheetLabel" data-datafield="DbcompanyProvinceName" style="">担保人公司省份</span>
                    </div>
                    <div id="control265" class="col-md-4">
                        <input id="Control253" type="text" data-datafield="DbcompanyProvinceName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title266" class="col-md-2">
                        <span id="Label254" data-type="SheetLabel" data-datafield="DbcompanyCityName" style="">担保人公司城市</span>
                    </div>
                    <div id="control266" class="col-md-4">
                        <input id="Control254" type="text" data-datafield="DbcompanyCityName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title267" class="col-md-2">
                        <span id="Label255" data-type="SheetLabel" data-datafield="Dbphonenumber" style="">担保人公司电话 </span>
                    </div>
                    <div id="control267" class="col-md-4">
                        <input id="Control255" type="text" data-datafield="Dbphonenumber" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title268" class="col-md-2">
                        <span id="Label256" data-type="SheetLabel" data-datafield="DbFax" style="">担保人公司传真 </span>
                    </div>
                    <div id="control268" class="col-md-4">
                        <input id="Control256" type="text" data-datafield="DbFax" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title269" class="col-md-2">
                        <span id="Label257" data-type="SheetLabel" data-datafield="DbcompanyAddress" style="">担保人公司地址</span>
                    </div>
                    <div id="control269" class="col-md-10">
                        <textarea id="Control257" data-datafield="DbcompanyAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row">
                    <div id="title271" class="col-md-2">
                        <span id="Label258" data-type="SheetLabel" data-datafield="DbcompanypostCode" style="">担保人公司邮编 </span>
                    </div>
                    <div id="control271" class="col-md-4">
                        <input id="Control258" type="text" data-datafield="DbcompanypostCode" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title272" class="col-md-2">
                        <span id="Label259" data-type="SheetLabel" data-datafield="DbEmployerType" style="">担保人雇主类型</span>
                    </div>
                    <div id="control272" class="col-md-4">
                        <input id="Control259" type="text" data-datafield="DbEmployerType" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title273" class="col-md-2">
                        <span id="Label260" data-type="SheetLabel" data-datafield="DbNoOfEmployees" style="">担保人雇员人数</span>
                    </div>
                    <div id="control273" class="col-md-4">
                        <input id="Control260" type="text" data-datafield="DbNoOfEmployees" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title274" class="col-md-2">
                        <span id="Label261" data-type="SheetLabel" data-datafield="DbJobDescription" style="">担保人工作描述</span>
                    </div>
                    <div id="control274" class="col-md-4">
                        <input id="Control261" type="text" data-datafield="DbJobDescription" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title275" class="col-md-2">
                        <span id="Label262" data-type="SheetLabel" data-datafield="Dbtimeinyear" style="">担保人工作年限（年）</span>
                    </div>
                    <div id="control275" class="col-md-4">
                        <input id="Control262" type="text" data-datafield="Dbtimeinyear" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title276" class="col-md-2">
                        <span id="Label263" data-type="SheetLabel" data-datafield="Dbtimeinmonth" style="">担保人工作年限（月） </span>
                    </div>
                    <div id="control276" class="col-md-4">
                        <input id="Control263" type="text" data-datafield="Dbtimeinmonth" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title277" class="col-md-2">
                        <span id="Label264" data-type="SheetLabel" data-datafield="DbComments" style="">担保人公司评论</span>
                    </div>
                    <div id="control277" class="col-md-10">
                        <textarea id="Control264" data-datafield="DbComments" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInf7',this)">
                <label data-en_us="Sheet information">担保人地址联系方式</label>
            </div>
            <div id="divBascSQRInf7">
                <div class="row tableContent">
                    <div id="title279" class="col-md-2">
                        <span id="Label265" data-type="SheetLabel" data-datafield="DbcurrentLivingAddress" style="">担保人地址</span>
                    </div>
                    <div id="control279" class="col-md-10">
                        <textarea id="Control265" data-datafield="DbcurrentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title281" class="col-md-2">
                        <span id="Label266" data-type="SheetLabel" data-datafield="DbAddressId" style="">担保人户籍地址</span>
                    </div>
                    <div id="control281" class="col-md-10">
                        <textarea id="Control266" data-datafield="DbAddressId" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row">
                    <div id="title283" class="col-md-2">
                        <span id="Label267" data-type="SheetLabel" data-datafield="Dbdefaultmailingaddress" style="">担保人默认邮寄地址</span>
                    </div>
                    <div id="control283" class="col-md-4">
                        <input id="Control267" type="text" data-datafield="Dbdefaultmailingaddress" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title284" class="col-md-2">
                        <span id="Label268" data-type="SheetLabel" data-datafield="Dbhukouaddress" style="">担保人户籍地址</span>
                    </div>
                    <div id="control284" class="col-md-4">
                        <input id="Control268" type="text" data-datafield="Dbhukouaddress" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title285" class="col-md-2">
                        <span id="Label269" data-type="SheetLabel" data-datafield="DbcountryName" style="">担保人国家</span>
                    </div>
                    <div id="control285" class="col-md-4">
                        <input id="Control269" type="text" data-datafield="DbcountryName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title286" class="col-md-2">
                        <span id="Label270" data-type="SheetLabel" data-datafield="Dbnativedistrict" style="">担保人籍贯</span>
                    </div>
                    <div id="control286" class="col-md-4">
                        <input id="Control270" type="text" data-datafield="Dbnativedistrict" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title287" class="col-md-2">
                        <span id="Label271" data-type="SheetLabel" data-datafield="Dbbirthpalaceprovince" style="">担保人出生地省市县（区） </span>
                    </div>
                    <div id="control287" class="col-md-4">
                        <input id="Control271" type="text" data-datafield="Dbbirthpalaceprovince" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title288" class="col-md-2">
                        <span id="Label272" data-type="SheetLabel" data-datafield="DbhukouprovinceName" style="">担保人省份</span>
                    </div>
                    <div id="control288" class="col-md-4">
                        <input id="Control272" type="text" data-datafield="DbhukouprovinceName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title289" class="col-md-2">
                        <span id="Label273" data-type="SheetLabel" data-datafield="DbhukoucityName" style="">担保人城市</span>
                    </div>
                    <div id="control289" class="col-md-4">
                        <input id="Control273" type="text" data-datafield="DbhukoucityName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title290" class="col-md-2">
                        <span id="Label274" data-type="SheetLabel" data-datafield="Dbpostcode" style="">担保人邮编</span>
                    </div>
                    <div id="control290" class="col-md-4">
                        <input id="Control274" type="text" data-datafield="Dbpostcode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title291" class="col-md-2">
                        <span id="Label275" data-type="SheetLabel" data-datafield="DbaddresstypeName" style="">担保人地址类型</span>
                    </div>
                    <div id="control291" class="col-md-4">
                        <input id="Control275" type="text" data-datafield="DbaddresstypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title292" class="col-md-2">
                        <span id="Label276" data-type="SheetLabel" data-datafield="DbaddressstatusName" style="">担保人地址状态</span>
                    </div>
                    <div id="control292" class="col-md-4">
                        <input id="Control276" type="text" data-datafield="DbaddressstatusName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title293" class="col-md-2">
                        <span id="Label277" data-type="SheetLabel" data-datafield="DbpropertytypeName" style="">担保人房产类型</span>
                    </div>
                    <div id="control293" class="col-md-4">
                        <input id="Control277" type="text" data-datafield="DbpropertytypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title294" class="col-md-2">
                        <span id="Label278" data-type="SheetLabel" data-datafield="DbresidencetypeName" style="">担保人住宅类型</span>
                    </div>
                    <div id="control294" class="col-md-4">
                        <input id="Control278" type="text" data-datafield="DbresidencetypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title295" class="col-md-2">
                        <span id="Label279" data-type="SheetLabel" data-datafield="Dblivingsince" style="">担保人开始居住日期</span>
                    </div>
                    <div id="control295" class="col-md-4">
                        <input id="Control279" type="text" data-datafield="Dblivingsince" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title296" class="col-md-2">
                        <span id="Label280" data-type="SheetLabel" data-datafield="Dbhomevalue" style="">担保人房屋价值（万元) </span>
                    </div>
                    <div id="control296" class="col-md-4">
                        <input id="Control280" type="text" data-datafield="Dbhomevalue" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title297" class="col-md-2">
                        <span id="Label281" data-type="SheetLabel" data-datafield="Dbstayinyear" style="">担保人居住年限</span>
                    </div>
                    <div id="control297" class="col-md-4">
                        <input id="Control281" type="text" data-datafield="Dbstayinyear" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title298" class="col-md-2">
                        <span id="Label282" data-type="SheetLabel" data-datafield="Dbstayinmonth" style="">担保人居住月份</span>
                    </div>
                    <div id="control298" class="col-md-4">
                        <input id="Control282" type="text" data-datafield="Dbstayinmonth" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title299" class="col-md-2">
                        <span id="Label283" data-type="SheetLabel" data-datafield="DbcountrycodeName" style="">担保人国家代码</span>
                    </div>
                    <div id="control299" class="col-md-4">
                        <input id="Control283" type="text" data-datafield="DbcountrycodeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title300" class="col-md-2">
                        <span id="Label284" data-type="SheetLabel" data-datafield="DbareaCode" style="">担保人地区代码</span>
                    </div>
                    <div id="control300" class="col-md-4">
                        <input id="Control284" type="text" data-datafield="DbareaCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title301" class="col-md-2">
                        <span id="Label285" data-type="SheetLabel" data-datafield="DbphoneNo" style="">担保人电话</span>
                    </div>
                    <div id="control301" class="col-md-4">
                        <input id="Control285" type="text" data-datafield="DbphoneNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title302" class="col-md-2">
                        <span id="Label286" data-type="SheetLabel" data-datafield="Dbextension" style="">担保人分机</span>
                    </div>
                    <div id="control302" class="col-md-4">
                        <input id="Control286" type="text" data-datafield="Dbextension" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title303" class="col-md-2">
                        <span id="Label287" data-type="SheetLabel" data-datafield="DbphonetypeName" style="">担保人电话类型</span>
                    </div>
                    <div id="control303" class="col-md-4">
                        <input id="Control287" type="text" data-datafield="DbphonetypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divZC',this)">
            <label id="Label3" data-en_us="Basic information">资产信息</label>
        </div>
        <div class="divContent" id="divZC">
            <div class="row">
                <div id="title304" class="col-md-2 light">
                    <span id="Label288" data-type="SheetLabel" data-datafield="assetConditionName" style="">资产状况</span>
                </div>
                <div id="control304" class="col-md-4 signcolor">
                    <input id="Control288" type="text" data-datafield="assetConditionName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title305" class="col-md-2 light">
                    <span id="Label289" data-type="SheetLabel" data-datafield="assetMakeName" style="">制造商</span>
                </div>
                <div id="control305" class="col-md-4 signcolor">
                    <input id="Control289" type="text" data-datafield="assetMakeName" data-type="SheetTextBox" style="">
                </div>
                <div id="title306" class="col-md-2 light">
                    <span id="Label290" data-type="SheetLabel" data-datafield="brandName" style="">车型</span>
                </div>
                <div id="control306" class="col-md-4 signcolor">
                    <input id="Control290" type="text" data-datafield="brandName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title307" class="col-md-2">
                    <span id="Label291" data-type="SheetLabel" data-datafield="modelName" style="">动力参数</span>
                </div>
                <div id="control307" class="col-md-4">
                    <input id="Control291" type="text" data-datafield="modelName" data-type="SheetTextBox" style="">
                </div>
                <div id="title308" class="col-md-2">
                    <span id="Label292" data-type="SheetLabel" data-datafield="MIOCN" style="">MIOCN</span>
                </div>
                <div id="control308" class="col-md-4">
                    <input id="Control292" type="text" data-datafield="MIOCN" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title309" class="col-md-2 light">
                    <span id="Label293" data-type="SheetLabel" data-datafield="assetPrice" style="">资产价格</span>
                </div>
                <div id="control309" class="col-md-4 signcolor">
                    <input id="Control293" type="text" data-datafield="assetPrice" data-type="SheetTextBox" style="">
                </div>
                <div id="title310" class="col-md-2 light">
                    <span id="Label294" data-type="SheetLabel" data-datafield="transmissionName" style="">变速器</span>
                </div>
                <div id="control310" class="col-md-4 signcolor">
                    <input id="Control294" type="text" data-datafield="transmissionName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title311" class="col-md-2">
                    <span id="Label295" data-type="SheetLabel" data-datafield="vehicleSubtypeName" style="">颜色</span>
                </div>
                <div id="control311" class="col-md-4">
                    <input id="Control295" type="text" data-datafield="vehicleSubtypeName" data-type="SheetTextBox" style="">
                </div>
                <div id="title312" class="col-md-2 light">
                    <span id="Label296" data-type="SheetLabel" data-datafield="purposeName" style="">购车目的</span>
                </div>
                <div id="control312" class="col-md-4 signcolor">
                    <input id="Control296" type="text" data-datafield="purposeName" data-type="SheetTextBox" style="">
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
                    <input id="Control298" type="text" data-datafield="vinNo" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title315" class="col-md-2">
                    <span id="Label299" data-type="SheetLabel" data-datafield="vehiclecolorName" style="">车身颜色</span>
                </div>
                <div id="control315" class="col-md-4">
                    <input id="Control299" type="text" data-datafield="vehiclecolorName" data-type="SheetTextBox" style="">
                </div>
                <div id="title316" class="col-md-2">
                    <span id="Label300" data-type="SheetLabel" data-datafield="vineditind" style="">编辑VIN号</span>
                </div>
                <div id="control316" class="col-md-4">
                    <input id="Control300" type="text" data-datafield="vineditind" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title317" class="col-md-2">
                    <span id="Label301" data-type="SheetLabel" data-datafield="manufactureYear" style="">Manufacture Year </span>
                </div>
                <div id="control317" class="col-md-4">
                    <input id="Control301" type="text" data-datafield="manufactureYear" data-type="SheetTextBox" style="">
                </div>
                <div id="title318" class="col-md-2">
                    <span id="Label302" data-type="SheetLabel" data-datafield="engineCC" style="">发动机排量</span>
                </div>
                <div id="control318" class="col-md-4">
                    <input id="Control302" type="text" data-datafield="engineCC" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title319" class="col-md-2 light">
                    <span id="Label303" data-type="SheetLabel" data-datafield="vehicleAge" style="">车辆使用年数</span>
                </div>
                <div id="control319" class="col-md-4 signcolor">
                    <input id="Control303" type="text" data-datafield="vehicleAge" data-type="SheetTextBox" style="">
                </div>
                <div id="title320" class="col-md-2">
                    <span id="Label304" data-type="SheetLabel" data-datafield="deliveryDate" style="">TBR日期</span>
                </div>
                <div id="control320" class="col-md-4">
                    <input id="Control304" type="text" data-datafield="deliveryDate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title321" class="col-md-2 light">
                    <span id="Label305" data-type="SheetLabel" data-datafield="releaseDate" style="">出厂日期</span>
                </div>
                <div id="control321" class="col-md-4 signcolor">
                    <input id="Control305" type="text" data-datafield="releaseDate" data-type="SheetTextBox" style="">
                </div>
                <div id="title322" class="col-md-2">
                    <span id="Label306" data-type="SheetLabel" data-datafield="releaseMonth" style="">出厂月份</span>
                </div>
                <div id="control322" class="col-md-4">
                    <input id="Control306" type="text" data-datafield="releaseMonth" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title323" class="col-md-2">
                    <span id="Label307" data-type="SheetLabel" data-datafield="releaseYear" style="">出厂年份</span>
                </div>
                <div id="control323" class="col-md-4">
                    <input id="Control307" type="text" data-datafield="releaseYear" data-type="SheetTextBox" style="">
                </div>
                <div id="title324" class="col-md-2">
                    <span id="Label308" data-type="SheetLabel" data-datafield="registrationNo" style="">注册号</span>
                </div>
                <div id="control324" class="col-md-4">
                    <input id="Control308" type="text" data-datafield="registrationNo" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title325" class="col-md-2">
                    <span id="Label309" data-type="SheetLabel" data-datafield="series" style="">系列</span>
                </div>
                <div id="control325" class="col-md-4">
                    <input id="Control309" type="text" data-datafield="series" data-type="SheetTextBox" style="">
                </div>
                <div id="title326" class="col-md-2">
                    <span id="Label310" data-type="SheetLabel" data-datafield="vehicleBody" style="">车身</span>
                </div>
                <div id="control326" class="col-md-4">
                    <input id="Control310" type="text" data-datafield="vehicleBody" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title327" class="col-md-2">
                    <span id="Label311" data-type="SheetLabel" data-datafield="style" style="">风格</span>
                </div>
                <div id="control327" class="col-md-4">
                    <input id="Control311" type="text" data-datafield="style" data-type="SheetTextBox" style="">
                </div>
                <div id="title328" class="col-md-2">
                    <span id="Label312" data-type="SheetLabel" data-datafield="cylinder" style="">汽缸</span>
                </div>
                <div id="control328" class="col-md-4">
                    <input id="Control312" type="text" data-datafield="cylinder" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title329" class="col-md-2 light">
                    <span id="Label313" data-type="SheetLabel" data-datafield="odometer" style="">里程数</span>
                </div>
                <div id="control329" class="col-md-4 signcolor">
                    <input id="Control313" type="text" data-datafield="odometer" data-type="SheetTextBox" style="">
                </div>
                <div id="title330" class="col-md-2 light">
                    <span id="Label314" data-type="SheetLabel" data-datafield="totalaccessoryamount" style="">轮宽（改为附加费）</span>
                </div>
                <div id="control330" class="col-md-4 signcolor">
                    <input id="Control314" type="text" data-datafield="totalaccessoryamount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title331" class="col-md-2">
                    <span id="Label315" data-type="SheetLabel" data-datafield="guranteeOption" style="">担保方式</span>
                </div>
                <div id="control331" class="col-md-4">
                    <input id="Control315" type="text" data-datafield="guranteeOption" data-type="SheetTextBox" style="">
                </div>
                <div id="space332" class="col-md-2">
                </div>
                <div id="spaceControl332" class="col-md-4">
                </div>
            </div>
            <div class="row tableContent">
                <div id="title333" class="col-md-2">
                    <span id="Label316" data-type="SheetLabel" data-datafield="vecomments" style="">车辆备注</span>
                </div>
                <div id="control333" class="col-md-10">
                    <textarea id="Control316" data-datafield="vecomments" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
        </div>
        <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divJRTK',this)">
            <label id="Label4" data-en_us="Basic information">金融条款</label>
        </div>
        <div class="divContent" id="divJRTK">
            <div class="row">
                <div id="title335" class="col-md-2 light">
                    <span id="Label317" data-type="SheetLabel" data-datafield="productGroupName" style="">产品组</span>
                </div>
                <div id="control335" class="col-md-4 signcolor">
                    <input id="Control317" type="text" data-datafield="productGroupName" data-type="SheetTextBox" style="">
                </div>
                <div id="title336" class="col-md-2 light">
                    <span id="Label318" data-type="SheetLabel" data-datafield="productTypeName" style="">产品类型</span>
                </div>
                <div id="control336" class="col-md-4 signcolor">
                    <input id="Control318" type="text" data-datafield="productTypeName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title337" class="col-md-2 light">
                    <span id="Label319" data-type="SheetLabel" data-datafield="paymentFrequencyName" style="">付款频率</span>
                </div>
                <div id="control337" class="col-md-4 signcolor">
                    <input id="Control319" type="text" data-datafield="paymentFrequencyName" data-type="SheetTextBox" style="">
                </div>
                <div id="title338" class="col-md-2">
                    <span id="Label320" data-type="SheetLabel" data-datafield="rentalmodeName" style="">租赁模式</span>
                </div>
                <div id="control338" class="col-md-4">
                    <input id="Control320" type="text" data-datafield="rentalmodeName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title339" class="col-md-2 light">
                    <span id="Label321" data-type="SheetLabel" data-datafield="termMonth" style="">贷款期数（月）</span>
                </div>
                <div id="control339" class="col-md-4 signcolor">
                    <input id="Control321" type="text" data-datafield="termMonth" data-type="SheetTextBox" style="">
                </div>
                <div id="title340" class="col-md-2 light">
                    <span id="Label322" data-type="SheetLabel" data-datafield="salesprice" style="">合同价格</span>
                </div>
                <div id="control340" class="col-md-4 signcolor">
                    <input id="Control322" type="text" data-datafield="salesprice" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title341" class="col-md-2">
                    <span id="Label323" data-type="SheetLabel" data-datafield="deferredTerm" style="">展期期数</span>
                </div>
                <div id="control341" class="col-md-4">
                    <input id="Control323" type="text" data-datafield="deferredTerm" data-type="SheetTextBox" style="">
                </div>
                <div id="title342" class="col-md-2 light">
                    <span id="Label324" data-type="SheetLabel" data-datafield="vehicleprice" style="">销售价格</span>
                </div>
                <div id="control342" class="col-md-4 signcolor">
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
                <div id="title344" class="col-md-2 light">
                    <span id="Label326" data-type="SheetLabel" data-datafield="downpaymentrate" style="">首付款比例 %</span>
                </div>
                <div id="control344" class="col-md-4 signcolor">
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
                <div id="title346" class="col-md-2 light">
                    <span id="Label328" data-type="SheetLabel" data-datafield="downpaymentamount" style="">首付款金额</span>
                </div>
                <div id="control346" class="col-md-4 signcolor">
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
                <div id="title348" class="col-md-2 light">
                    <span id="Label330" data-type="SheetLabel" data-datafield="financedamount" style="">贷款金额</span>
                </div>
                <div id="control348" class="col-md-4 signcolor">
                    <input id="ctrlfinancedamount" type="text" data-datafield="financedamount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title349" class="col-md-2">
                    <span id="Label331" data-type="SheetLabel" data-datafield="subsidyAmount" style="">贴息金额</span>
                </div>
                <div id="control349" class="col-md-4">
                    <input id="Control331" type="text" data-datafield="subsidyAmount" data-type="SheetTextBox" style="">
                </div>
                <div id="title350" class="col-md-2 light">
                    <span id="Label332" data-type="SheetLabel" data-datafield="financedamountrate" style="">贷款额比例%</span>
                </div>
                <div id="control350" class="col-md-4 signcolor">
                    <input id="Control332" type="text" data-datafield="financedamountrate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title351" class="col-md-2">
                    <span id="Label333" data-type="SheetLabel" data-datafield="otherfees" style="">其他费用</span>
                </div>
                <div id="control351" class="col-md-4">
                    <input id="Control333" type="text" data-datafield="otherfees" data-type="SheetTextBox" style="">
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
                    <input id="Control335" type="text" data-datafield="totalintamount" data-type="SheetTextBox" style="">
                </div>
                <div id="title354" class="col-md-2">
                    <span id="Label336" data-type="SheetLabel" data-datafield="subsidyRate" style="">贴息利率</span>
                </div>
                <div id="control354" class="col-md-4">
                    <input id="Control336" type="text" data-datafield="subsidyRate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title355" class="col-md-2">
                    <span id="Label337" data-type="SheetLabel" data-datafield="totalamountpayable" style="">应付总额</span>
                </div>
                <div id="control355" class="col-md-4">
                    <input id="Control337" type="text" data-datafield="totalamountpayable" data-type="SheetTextBox" style="">
                </div>
                <div id="title356" class="col-md-2">
                    <span id="Label338" data-type="SheetLabel" data-datafield="additionalName" style="">是否有附加费</span>
                </div>
                <div id="control356" class="col-md-4">
                    <input id="Control338" type="text" data-datafield="additionalName" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title357" class="col-md-2">
                    <span id="Label339" data-type="SheetLabel" data-datafield="price" style="">价格</span>
                </div>
                <div id="control357" class="col-md-4">
                    <input id="Control339" type="text" data-datafield="price" data-type="SheetTextBox" style="">
                </div>
                <div id="title358" class="col-md-2">
                    <span id="Label340" data-type="SheetLabel" data-datafield="BalancePayable" style="">未偿余额</span>
                </div>
                <div id="control358" class="col-md-4">
                    <input id="Control340" type="text" data-datafield="BalancePayable" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title3571" class="col-md-2 light">
                    <span id="Label3391" data-type="SheetLabel" data-datafield="ckje" style="">敞口金额</span>
                </div>
                <div id="control3571" class="col-md-4 signcolor">
                    <span id="ctrlCkje" data-type="SheetLabel" data-datafield="ckje" style=""></span>
                </div>
                <div id="title3581" class="col-md-2">
                    <span id="Label3401" data-type="SheetLabel" data-datafield="BalancePayable" style=""><a href="#example" data-toggle="modal" onclick="" target="_blank">详细</a></span>
                </div>
                <div id="control3581" class="col-md-4">
                </div>
            </div>
            <div class="row">
                <div id="title407" class="col-md-2">
                    <span id="Label366" data-type="SheetLabel" data-datafield="Bankname" style="">客户银行名称</span>
                </div>
                <div id="control407" class="col-md-4">
                    <%--<select data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>--%>
                    <input data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME" />
                </div>
                <div id="title408" class="col-md-2">
                    <span id="Label367" data-type="SheetLabel" data-datafield="Branchname" style="">客户银行分支</span>
                </div>
                <div id="control408" class="col-md-4">
                    <%--<select data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME"></select>--%>
                    <input data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME" />
                </div>
            </div>
            <div class="row">
                <div id="title409" class="col-md-2">
                    <span id="Label368" data-type="SheetLabel" data-datafield="Accoutname" style="">客户账户名</span>
                </div>
                <div id="control409" class="col-md-4">
                    <input id="Control368" type="text" data-datafield="Accoutname" data-type="SheetTextBox" style="">
                </div>
                <div id="title410" class="col-md-2">
                    <span id="Label369" data-type="SheetLabel" oninput="textnum(this)" data-datafield="AccoutNumber" style="">客户账户号</span>
                </div>
                <div id="control410" class="col-md-4">
                    <input id="Control369" type="text" data-datafield="AccoutNumber" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row tableContent">
                <div id="title359" class="col-md-2">
                    <span id="Label341" data-type="SheetLabel" data-datafield="RentalDetailtable" style="">还款计划</span>
                </div>
                <div id="control359" class="col-md-10">
                    <table id="Control341" data-datafield="RentalDetailtable" data-type="SheetGridView" class="SheetGridView">
                        <tbody>

                            <tr class="header">
                                <td id="Control341_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control341_Header3" data-datafield="RentalDetailtable.startTerm">
                                    <label id="Control341_Label3" data-datafield="RentalDetailtable.startTerm" data-type="SheetLabel" style="">还款起始期</label>
                                </td>
                                <td id="Control341_Header4" data-datafield="RentalDetailtable.endTerm">
                                    <label id="Control341_Label4" data-datafield="RentalDetailtable.endTerm" data-type="SheetLabel" style="">还款结束期</label>
                                </td>
                                <td id="Control341_Header5" data-datafield="RentalDetailtable.rentalAmount">
                                    <label id="Control341_Label5" data-datafield="RentalDetailtable.rentalAmount" data-type="SheetLabel" style="">还款额</label>
                                </td>
                                <td id="Control341_Header6" data-datafield="RentalDetailtable.grossRentalAmount">
                                    <label id="Control341_Label6" data-datafield="RentalDetailtable.grossRentalAmount" data-type="SheetLabel" style="">每期还款总额</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control341_Option" class="rowOption"></td>
                                <td data-datafield="RentalDetailtable.startTerm">
                                    <input id="Control341_ctl3" type="text" data-datafield="RentalDetailtable.startTerm" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="RentalDetailtable.endTerm">
                                    <input id="Control341_ctl4" type="text" data-datafield="RentalDetailtable.endTerm" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="RentalDetailtable.rentalAmount">
                                    <input id="Control341_ctl5" type="text" data-datafield="RentalDetailtable.rentalAmount" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="RentalDetailtable.grossRentalAmount">
                                    <input id="Control341_ctl6" type="text" data-datafield="RentalDetailtable.grossRentalAmount" data-type="SheetTextBox" style="">
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
                                <td data-datafield="RentalDetailtable.startTerm"></td>
                                <td data-datafield="RentalDetailtable.endTerm"></td>
                                <td data-datafield="RentalDetailtable.rentalAmount"></td>
                                <td data-datafield="RentalDetailtable.grossRentalAmount"></td>
                                <td class="rowOption"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divzcfz',this)">
                <label data-en_us="Sheet information">资产/负债</label>
            </div>
            <div id="divzcfz">
                <div class="row">
                    <div id="title108" class="col-md-2">
                        <span id="Label108" data-type="SheetLabel" data-datafield="Descritption" style="">收入</span>
                    </div>
                    <div id="control108" class="col-md-4">
                        <input id="Control108" type="text" data-datafield="Descritption" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <%--<div class="row">
                    <div id="title109" class="col-md-2">
                        <span id="Label109" data-type="SheetLabel" data-datafield="Descritption2" style="">支出</span>
                    </div>
                    <div id="control109" class="col-md-4">
                        <input id="Control109" type="text" data-datafield="Descritption2" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title110" class="col-md-2">
                        <span id="Label110" data-type="SheetLabel" data-datafield="Value2" style="">金额</span>
                    </div>
                    <div id="control110" class="col-md-4">
                        <input id="Control110" type="text" data-datafield="Value2" data-type="SheetTextBox" style="">
                    </div>
                </div>--%>
                <div class="row">
                    <div id="title111" class="col-md-2">
                        <span id="Label111" data-type="SheetLabel" data-datafield="Financialratiodes" style="">财务比率</span>
                    </div>
                    <div id="control111" class="col-md-4">
                        <input id="Control111" type="text" data-datafield="Financialratiodes" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title112" class="col-md-2">
                        <span id="Label112" data-type="SheetLabel" data-datafield="Evalresult" style="">财务比率金额</span>
                    </div>
                    <div id="control112" class="col-md-4">
                        <input id="Control112" type="text" data-datafield="Evalresult" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title429" class="col-md-2">
                        <span id="Label384" data-type="SheetLabel" data-datafield="Dfgz" style="">代发工资</span>
                    </div>
                    <div id="control429" class="col-md-4">
                        <input id="Control384" type="text" data-datafield="Dfgz" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title430" class="col-md-2">
                        <span id="Label385" data-type="SheetLabel" data-datafield="Dqck" style="">定期存款</span>
                    </div>
                    <div id="control430" class="col-md-4">
                        <input id="Control385" type="text" data-datafield="Dqck" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title431" class="col-md-2">
                        <span id="Label386" data-type="SheetLabel" data-datafield="Gp" style="">股票</span>
                    </div>
                    <div id="control431" class="col-md-4">
                        <input id="Control386" type="text" data-datafield="Gp" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title432" class="col-md-2">
                        <span id="Label387" data-type="SheetLabel" data-datafield="Hqjx" style="">活期结息</span>
                    </div>
                    <div id="control432" class="col-md-4">
                        <input id="Control387" type="text" data-datafield="Hqjx" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title433" class="col-md-2">
                        <span id="Label388" data-type="SheetLabel" data-datafield="Gj" style="">基金</span>
                    </div>
                    <div id="control433" class="col-md-4">
                        <input id="Control388" type="text" data-datafield="Gj" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title434" class="col-md-2">
                        <span id="Label389" data-type="SheetLabel" data-datafield="Lc" style="">理财</span>
                    </div>
                    <div id="control434" class="col-md-4">
                        <input id="Control389" type="text" data-datafield="Lc" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title435" class="col-md-2">
                        <span id="Label390" data-type="SheetLabel" data-datafield="Bccdygje" style="">本次车贷月供金额</span>
                    </div>
                    <div id="control435" class="col-md-4">
                        <input id="Control390" type="text" data-datafield="Bccdygje" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title436" class="col-md-2">
                        <span id="Label391" data-type="SheetLabel" data-datafield="QTFZYG" style="">其他负债月供</span>
                    </div>
                    <div id="control436" class="col-md-4">
                        <input id="Control391" type="text" data-datafield="QTFZYG" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title437" class="col-md-2">
                        <span id="Label392" data-type="SheetLabel" data-datafield="HQRJ" style="">活期日均</span>
                    </div>
                    <div id="control437" class="col-md-4">
                        <input id="Control392" type="text" data-datafield="HQRJ" data-type="SheetTextBox" style="" class="" data-computationrule="2,{Hqjx}*4/0.0035">
                    </div>
                    <div id="title438" class="col-md-2">
                        <span id="Label393" data-type="SheetLabel" data-datafield="JRZC" style="">金融资产</span>
                    </div>
                    <div id="control438" class="col-md-4">
                        <input id="Control393" type="text" data-datafield="JRZC" data-type="SheetTextBox" style="" data-computationrule="2,{Hqjx}*4/0.0035*0.7 +{Dqck} *0.7 +{Lc}*0.5 + {Gp} * 0.4 + {Gj}*0.5">
                    </div>
                </div>
                <div class="row">
                    <div id="title439" class="col-md-2 light">
                        <span id="Label394" data-type="SheetLabel" data-datafield="SRFZB" style="">收入负债比</span>
                    </div>
                    <div id="control439" class="col-md-4 signcolor">
                        <input id="Control394" type="text" data-datafield="SRFZB" data-type="SheetTextBox" style="" data-computationrule="2,({Bccdygje}+{QTFZYG})/({Dfgz}*1.2+({Hqjx}*4/0.0035*0.7 + {Dqck} *0.7 + {Lc}*0.5 + {Gp} * 0.4 + {Gj}*0.5)/2)">
                    </div>
                    <div id="space440" class="col-md-2">
                    </div>
                    <div id="spaceControl440" class="col-md-4">
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divattachment',this)">
            <label id="Label1" data-en_us="Sheet information">附件信息</label>
            <a href="javascript:void(0);" onclick="getDownLoadURL()">附件详情</a>
        </div>
        <div class="divContent" id="divattachment">
            <div id="divfjll">
                <ul id="ulfjll">
                </ul>
            </div>
            <div class="row">
                <div id="title361" class="col-md-2">
                    <span id="Label342" data-type="SheetLabel" data-datafield="SQD" style="">申请单资料</span>
                </div>
                <div id="control361" class="col-md-10">
                    <div id="Control342" data-datafield="SQD" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title375" class="col-md-2">
                    <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
                </div>
                <div id="control375" class="col-md-10">
                    <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title363" class="col-md-2">
                    <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
                </div>
                <div id="control363" class="col-md-10">
                    <div id="Control343" data-datafield="SFZ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title367" class="col-md-2">
                    <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
                </div>
                <div id="control367" class="col-md-10">
                    <div id="Control345" data-datafield="JSZ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title365" class="col-md-2">
                    <span id="Label344" data-type="SheetLabel" data-datafield="JHZ" style="">婚姻类材料</span>
                </div>
                <div id="control365" class="col-md-10">
                    <div id="Control344" data-datafield="JHZ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title369" class="col-md-2">
                    <span id="Label346" data-type="SheetLabel" data-datafield="JZZ" style="">居住类材料</span>
                </div>
                <div id="control369" class="col-md-10">
                    <div id="Control346" data-datafield="JZZ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title373" class="col-md-2">
                    <span id="Label348" data-type="SheetLabel" data-datafield="GZ" style="">工作证明\企业类证明</span>
                </div>
                <div id="control373" class="col-md-10">
                    <div id="Control348" data-datafield="GZ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title371" class="col-md-2">
                    <span id="Label347" data-type="SheetLabel" data-datafield="SR" style="">收入类材料</span>
                </div>
                <div id="control371" class="col-md-10">
                    <div id="Control347" data-datafield="SR" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title391" class="col-md-2">
                    <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">银行卡</span>
                </div>
                <div id="control391" class="col-md-10">
                    <div id="Control357" data-datafield="YHKMFYJ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title471" class="col-md-2">
                    <span id="Label420" data-type="SheetLabel" data-datafield="xsqtfj" style="">信审其他附件</span>
                </div>
                <div id="control471" class="col-md-10">
                    <div id="Control420" data-datafield="xsqtfj" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
            <label id="Label2" data-en_us="Sheet information">审核信息</label>
        </div>
        <div class="divContent" id="div2">
            <div class="row tableContent">
                <div id="title427" class="col-md-2">
                    <span id="Label383" data-type="SheetLabel" data-datafield="NBLYB" style="">内部留言板</span>
                </div>
                <div id="control427" class="col-md-10">
                    <div class="col-md-9">
                        <textarea id="Control383" data-datafield="NBLYB" data-type="SheetRichTextBox" style=""></textarea>
                    </div>
                    <div id="control467" class="col-md-1">
                        <input id="Control418" type="checkbox" data-datafield="ccbh" data-type="SheetCheckbox" style="">
                    </div>
                    <div id="title467" class="col-md-2">
                        <span id="Label418" data-type="SheetLabel" data-datafield="ccbh" style="">差错驳回</span>
                    </div>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title425" class="col-md-2">
                    <span id="Label382" data-type="SheetLabel" data-datafield="THJL" style="">电调记录</span>
                </div>
                <div id="control425" class="col-md-10">
                    <textarea id="Control382" data-datafield="THJL" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title399" class="col-md-2">
                    <span id="Label361" data-type="SheetLabel" data-datafield="CSYJ" style="">初审意见</span>
                </div>
                <div id="control399" class="col-md-10">
                    <div data-datafield="csshzt" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultitems="核准;拒绝;有条件核准;取消"></div>
                    <div id="Control361" data-datafield="CSYJ" data-type="SheetComment" style="">
                    </div>
                </div>
            </div>
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
            <%--<div class="row tableContent">
                <div id="title423" class="col-md-2">
                    <span id="Label381" data-type="SheetLabel" data-datafield="LYB" style="">留言板</span>
                </div>
                <div id="control423" class="col-md-10">
                    <textarea id="Control381" data-datafield="LYB" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>--%>
        </div>
    </div>
    <div id="example" class="modal fade in" aria-hidden="true">
        <div class="modal-header">
            <a class="close" data-dismiss="modal">×</a>
            <h3 style="text-align: center;">风险敞口</h3>
        </div>
        <div class="modal-body">
            <div style="text-align:left;padding:0 5px;font-weight:700;background-color:#ccc;height:30px;line-height:30px;">CAP：风险敞口</div>
            <div style="height:auto;min-height:90px;max-height:160px;overflow-y:scroll;">
            <table class="SheetGridView" style="width:1500px;">
                <thead>
                    <tr class="header">
                        <td id="" class="rowSerialNo"></td>
                        <td id="Td1">
                            <label id="Label26" style="">融资额</label>
                        </td>
                        <td id="Td2">
                            <label id="Label109" style="">申请号</label>
                        </td>
                        <td id="Td3">
                            <label id="Label110" style="">状态</label>
                        </td>
                        <td id="Td4">
                            <label id="Label204" style="">角色</label>
                        </td>
                        <td id="Td5">
                            <label id="Label205" style="">ID/注册号</label>
                        </td>
                        <td id="Td6">
                            <label id="Label206" style="">申请人姓名</label>
                        </td>
                        <td id="Td7">
                            <label id="Label207" style="">融资类型</label>
                        </td>
                        <td id="Td8">
                            <label id="Label208" style="">类型</label>
                        </td>
                        <td id="Td20">
                            <label id="Label350" style="">状态日期</label>
                        </td>
                        <td id="Td21">
                            <label id="Label364" style="">期数</label>
                        </td>
                        <td id="Td22">
                            <label id="Label365" style="">生产商</label>
                        </td>
                        <td id="Td23">
                            <label id="Label370" style="">动力参数</label>
                        </td>
                        <td id="Td24">
                            <label id="Label371" style="">车型</label>
                        </td>
                    </tr>
                </thead>
                <tbody id="Application" style="text-align: center;"></tbody>
            </table>
<br />
            <table class="SheetGridView" style="width:1500px;">
                <thead>
                    <tr class="header">
                        <td id="Td38" class="rowSerialNo"></td>
                        <td id="Td39">
                            <label id="Label400" style="">融资额</label>
                        </td>
                        <td id="Td40">
                            <label id="Label401" style="">申请号</label>
                        </td>
                        <td id="Td41">
                            <label id="Label402" style="">状态</label>
                        </td>
                        <td id="Td42">
                            <label id="Label403" style="">角色</label>
                        </td>
                        <td id="Td43">
                            <label id="Label404" style="">ID/注册号</label>
                        </td>
                        <td id="Td44">
                            <label id="Label405" style="">申请人姓名</label>
                        </td>
                        <td id="Td45">
                            <label id="Label406" style="">融资类型</label>
                        </td>
                        <td id="Td46">
                            <label id="Label407" style="">类型</label>
                        </td>
                        <td id="Td47">
                            <label id="Label408" style="">状态日期</label>
                        </td>
                        <td id="Td48">
                            <label id="Label409" style="">期数</label>
                        </td>
                        <td id="Td49">
                            <label id="Label410" style="">生产商</label>
                        </td>
                        <td id="Td50">
                            <label id="Label411" style="">动力参数</label>
                        </td>
                        <td id="Td51">
                            <label id="Label412" style="">车型</label>
                        </td>
                    </tr>
                </thead>
                <tbody id="ApplicationGJR" style="text-align: center;"></tbody>
            </table>
            </div>
            <div style="text-align:left;padding:0 5px;font-weight:700;background-color:#ccc;height:30px;line-height:30px;">CMS：风险敞口</div>
            <div style="height:auto;min-height:90px;max-height:160px;overflow:scroll;">
            <table class="SheetGridView"  style="width:2400px;">
                <thead>
                    <tr class="header">
                        <td id="Td9" class="rowSerialNo"></td>
                        <td id="Td10">
                            <label id="Label351" style="">本金余额</label>
                        </td>
                        <td id="Td11">
                            <label id="Label352" style="">申请号</label>
                        </td>
                        <td id="Td12">
                            <label id="Label353" style="">合同号</label>
                        </td>
                        <td id="Td13">
                            <label id="Label354" style="">状态</label>
                        </td>
                        <td id="Td14">
                            <label id="Label355" style="">申请人状态</label>
                        </td>
                        <td id="Td15">
                            <label id="Label356" style="">角色</label>
                        </td>
                        <td id="Td16">
                            <label id="Label358" style="">ID/注册号</label>
                        </td>
                        <td id="Td17">
                            <label id="Label359" style="">姓名</label>
                        </td>
                        <td id="Td18">
                            <label id="Label360" style="">经销商</label>
                        </td>
                        <td id="Td19">
                            <label id="Label363" style="">合同类型</label>
                        </td>
                        <td id="Td25">
                            <label id="Label372" style="">合同日期</label>
                        </td>
                        <td id="Td26">
                            <label id="Label373" style="">利率</label>
                        </td>
                        <td id="Td27">
                            <label id="Label374" style="">融资额</label>
                        </td>
                        <td id="Td28">
                            <label id="Label375" style="">生产商</label>
                        </td>
                        <td id="Td29">
                            <label id="Label376" style="">动力参数</label>
                        </td>
                        <td id="Td30">
                            <label id="Label377" style="">车型</label>
                        </td>
                        <td id="Td31">
                            <label id="Label378" style="">30天</label>
                        </td>
                        <td id="Td32">
                            <label id="Label379" style="">60天</label>
                        </td>
                        <td id="Td33">
                            <label id="Label381" style="">90天</label>
                        </td>
                        <td id="Td34">
                            <label id="Label395" style="">90天以上</label>
                        </td>
                        <td id="Td35">
                            <label id="Label396" style="">120天以上</label>
                        </td>
                        <td id="Td36">
                            <label id="Label397" style="">期数</label>
                        </td>
                        <td id="Td37">
                            <label id="Label399" style="">已付期数</label>
                        </td>
                    </tr>
                </thead>
                <tbody id="cmsApplication" style="text-align: center;"></tbody>
            </table>
<br />
            <table class="SheetGridView"  style="width:2400px;">
                <thead>
                    <tr class="header">
                        <td id="Td52" class="rowSerialNo"></td>
                        <td id="Td53">
                            <label id="Label413" style="">本金余额</label>
                        </td>
                        <td id="Td54">
                            <label id="Label414" style="">申请号</label>
                        </td>
                        <td id="Td55">
                            <label id="Label415" style="">合同号</label>
                        </td>
                        <td id="Td56">
                            <label id="Label416" style="">状态</label>
                        </td>
                        <td id="Td57">
                            <label id="Label417" style="">申请人状态</label>
                        </td>
                        <td id="Td58">
                            <label id="Label419" style="">角色</label>
                        </td>
                        <td id="Td59">
                            <label id="Label421" style="">ID/注册号</label>
                        </td>
                        <td id="Td60">
                            <label id="Label422" style="">姓名</label>
                        </td>
                        <td id="Td61">
                            <label id="Label423" style="">经销商</label>
                        </td>
                        <td id="Td62">
                            <label id="Label424" style="">合同类型</label>
                        </td>
                        <td id="Td63">
                            <label id="Label425" style="">合同日期</label>
                        </td>
                        <td id="Td64">
                            <label id="Label426" style="">利率</label>
                        </td>
                        <td id="Td65">
                            <label id="Label427" style="">融资额</label>
                        </td>
                        <td id="Td66">
                            <label id="Label428" style="">生产商</label>
                        </td>
                        <td id="Td67">
                            <label id="Label429" style="">动力参数</label>
                        </td>
                        <td id="Td68">
                            <label id="Label430" style="">车型</label>
                        </td>
                        <td id="Td69">
                            <label id="Label431" style="">30天</label>
                        </td>
                        <td id="Td70">
                            <label id="Label432" style="">60天</label>
                        </td>
                        <td id="Td71">
                            <label id="Label433" style="">90天</label>
                        </td>
                        <td id="Td72">
                            <label id="Label434" style="">90天以上</label>
                        </td>
                        <td id="Td73">
                            <label id="Label435" style="">120天以上</label>
                        </td>
                        <td id="Td74">
                            <label id="Label436" style="">期数</label>
                        </td>
                        <td id="Td75">
                            <label id="Label437" style="">已付期数</label>
                        </td>
                    </tr>
                </thead>
                <tbody id="cmsApplication1" style="text-align: center;"></tbody>
            </table>
            </div>
        </div>
        <div class="modal-footer" style="position: absolute; right: 0; top: 445px; width: 100%; height: 50px;padding:5px 20px;">
            <span style="font-weight:600;float:left;height:20px;line-height:20px;">总敞口金额：</span>
            <span id="fullckje" style="display:inline-block;float:left;min-width:100px;max-width:150px;height:20px;line-height:20px;border:1px solid lightgray;text-align:center;"></span>
            <span style="font-weight:600;float:left;height:20px;line-height:20px;padding-left:10px;">信贷申请系统敞口金额：</span>
            <span id="capckje" style="display:inline-block;float:left;min-width:100px;max-width:150px;height:20px;line-height:20px;border:1px solid lightgray;text-align:center;"></span>
            <span style="font-weight:600;float:left;height:20px;line-height:20px;padding-left:10px;">贷后管理系统敞口金额：</span>
            <span id="cmsckje" style="display:inline-block;float:left;min-width:100px;max-width:150px;height:20px;line-height:20px;border:1px solid lightgray;text-align:center;"></span>
<div style="position:absolute;left:8px;top:30px;">
<span style="font-weight:600;float:left;height:20px;line-height:20px;padding-left:10px;">共借人/担保人信贷敞口：</span>
            <span id="gjrcapck" style="display:inline-block;float:left;min-width:100px;max-width:150px;height:20px;line-height:20px;border:1px solid lightgray;text-align:center;"></span>
<span style="font-weight:600;float:left;height:20px;line-height:20px;padding-left:10px;">共借人/担保人贷后敞口：</span>
            <span id="gjrcmsck" style="display:inline-block;float:left;min-width:100px;max-width:150px;height:20px;line-height:20px;border:1px solid lightgray;text-align:center;"></span>
</div>
            <a class="btn" data-dismiss="modal">关闭</a>
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
            //获取敞口值
            //getCkje();
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
        //附件详情
        function getDownLoadURL() {
            if ($("#divattachment").find("tr").length == 0) {
                alert("附件为空！");
            } else {
                window.open("../view/FI.html");
            }
            event.stopPropagation();
        }
        var viewer;
        function getCkje() {
            debugger;
            var IdCardNo = $("#ctrlIdCardNo").val();
            var applicationno = $("#applicationNo").val();
            //var IdCardNo = "152626198901026716";
            //var applicationno = "Br-A058181000";
            if (!IdCardNo || IdCardNo == "" || !applicationno || applicationno == "" ) {
                return false;
            }
            var capckje = 0;
            var gjrcapck = 0;
            var gjrcmsck = 0;
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getCAPckje",// 19.6.28 wangxg
                data: { CommandName: "getCAPckje", applicantcardid: IdCardNo, applicationnumber: applicationno, cap: "cap" },
                type: "POST",
                dataType: "json",
                success: function (result) {
                    var arr = [];
                    arr['01'] = "核准";
                    arr['02'] = "拒绝";
                    arr['03'] = "有条件核准";
                    arr['04'] = "取消";
                    arr['05'] = "新的";
                    arr['97'] = "驳回";
                    for (var i = 0; i < result.length; i++) {
                        var tr = "<tr>";
                        if (result[i].EXP_APPLICATION_NUMBER == applicationno) {
                            capckje = capckje * 1 + result[i]['ROUND(CE.NET_FINANCED_AMT)'] * 1;
                            continue;
                        }
                        if (result[i].IS_EXPOSED_IND == "T") {
                            tr += "<td><input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/></td>";
                            capckje =capckje*1 + result[i]['ROUND(CE.NET_FINANCED_AMT)']*1;
                        } else {
                                tr += "<td><input onchange=\"capcheck(this,'capckje')\" data-number=\"" + result[i]['ROUND(CE.NET_FINANCED_AMT)'] + "\" type=\"checkbox\"/></td>";
                        }
                        tr += "<td>" + result[i]['ROUND(CE.NET_FINANCED_AMT)'] + "</td>";
                        tr += "<td><a href=\"javascript:void(0);\" onclick=\"getinstence('" + result[i].EXP_APPLICATION_NUMBER + "')\" >" + result[i].EXP_APPLICATION_NUMBER + "</a></td>";
                        if (arr.hasOwnProperty(result[i].APPLICATION_STATUS_CDE)) {
                            tr += "<td>" + arr[result[i].APPLICATION_STATUS_CDE] + "</td>";
                        } else {
                            tr += "<td>" + result[i].APPLICATION_STATUS_CDE + "</td>";
                        }
                        tr += "<td>借款人</td>";
                        tr += "<td>" + result[i].EXP_APPLICANT_CARD_ID + "</td>";
                        tr += "<td>" + result[i].EXP_APPLICANT_NAME + "</td>";
                        tr += "<td>" + result[i].FP_GROUP_NME + "</td>";
                        //if (result[i].EXP_APPLICATION_NUMBER == applicationno) {
                        //    tr += "<td>This Request</td>";
                        //} else {
                            tr += "<td>Old Request</td>";
                        //}
                        tr += "<td>" + result[i].APPLICATION_STATUS_DTE.split(" ")[0] + "</td>";
                        tr += "<td>" + result[i].NO_OF_TERMS + "</td>";
                        tr += "<td>" + result[i].ASSET_MAKE_DSC + "</td>";
                        tr += "<td>" + result[i].ASSET_MODEL_DSC + "</td>";
                        tr += "<td>" + result[i].ASSET_BRAND_DSC + "</td>";
                        tr += "</tr>";
                        $("#Application").append(tr);
                    }
                    //$("#ctrlCkje").html(result.Result);
                    $("#capckje").html(capckje);
                    console.log(result);
                    getCMSCKJE();
                },
                //error: function (msg) {
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
$.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getGJRck",// 19.6.28 wangxg
                data: { CommandName: "getGJRck", applicantcardid: IdCardNo, applicationnumber: applicationno, cap: "cap" },
                type: "POST",
                dataType: "text",
                success: function (res) {
                    var result = eval("(" + res.split("getGJRck:")[1] + ")");
                    console.log(result);
                    var arr = [];
                    arr['01'] = "核准";
                    arr['02'] = "拒绝";
                    arr['03'] = "有条件核准";
                    arr['04'] = "取消";
                    arr['05'] = "新的";
                    arr['97'] = "驳回";
                    for (var i = 0; i < result.length; i++) {
                        var tr = "<tr>";
                        if (result[i].IS_EXPOSED_IND == "T") {
                            tr += "<td><input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/></td>";
                            gjrcapck = gjrcapck * 1 + result[i]['ROUND(CE.NET_FINANCED_AMT)'] * 1;
                        } else {
                            tr += "<td><input onchange=\"capcheck(this,'capckje')\" data-number=\"" + result[i]['ROUND(CE.NET_FINANCED_AMT)'] + "\" type=\"checkbox\"/></td>";
                        }
                        tr += "<td>" + result[i]['ROUND(CE.NET_FINANCED_AMT)'] + "</td>";
                        tr += "<td><a href=\"javascript:void(0);\" onclick=\"getinstence('" + result[i].EXP_APPLICATION_NUMBER + "')\" >" + result[i].EXP_APPLICATION_NUMBER + "</a></td>";
                        if (arr.hasOwnProperty(result[i].APPLICATION_STATUS_CDE)) {
                            tr += "<td>" + arr[result[i].APPLICATION_STATUS_CDE] + "</td>";
                        } else {
                            tr += "<td>" + result[i].APPLICATION_STATUS_CDE + "</td>";
                        }
                        if (result[i].IDENTIFICATION_CODE == 2) {
                            tr += "<td>共借人</td>";
                        } else {
                            tr += "<td>担保人</td>";
                        }
                        tr += "<td>" + result[i].EXP_APPLICANT_CARD_ID + "</td>";
                        tr += "<td>" + result[i].EXP_APPLICANT_NAME + "</td>";
                        tr += "<td>" + result[i].FP_GROUP_NME + "</td>";
                        tr += "<td>Old Request</td>";
                        tr += "<td>" + result[i].APPLICATION_STATUS_DTE.split(" ")[0] + "</td>";
                        tr += "<td>" + result[i].NO_OF_TERMS + "</td>";
                        tr += "<td>" + result[i].ASSET_MAKE_DSC + "</td>";
                        tr += "<td>" + result[i].ASSET_MODEL_DSC + "</td>";
                        tr += "<td>" + result[i].ASSET_BRAND_DSC + "</td>";
                        tr += "</tr>";
                        $("#ApplicationGJR").append(tr);
                    }
                    $("#gjrcapck").html(gjrcapck);
                    var request = eval('(' + res.split("getGJRck:")[2] + ')');
                    console.log(request);
                    for (var i = 0; i < request.length; i++) {
                        var tr = "<tr>";
                        if (request[i].IS_EXPOSED_IND == "T") {
                            tr += "<td><input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/></td>";
                            gjrcmsck = gjrcmsck * 1 + request[i]['ROUND(CME.PRINCIPLE_OUTSTANDING_AMT)'] * 1;
                        } else {
                            tr += "<td><input disabled=\"disabled\" type=\"checkbox\"/></td>";
                        }
                        tr += "<td>" + request[i]['ROUND(CME.PRINCIPLE_OUTSTANDING_AMT)'] + "</td>";
                        tr += "<td><a href=\"javascript:void(0);\" onclick=\"getinstence('" + request[i].EXP_APPLICATION_NUMBER + "')\" >" + request[i].EXP_APPLICATION_NUMBER + "</a></td>";
                        tr += "<td>" + request[i].CONTRACT_NUMBER + "</td>";
                        tr += "<td>" + request[i].REQUEST_STATUS_DSC + "</td>";
                        var active = request[i].CONTRACT_STATUS_CDE == "20" ? "active" : request[i].CONTRACT_STATUS_CDE;
                        tr += "<td>" + active + "</td>";
                        //var sqr = request[i].IDENTIFICATION_CODE == "1" ? "借款人" : request[i].IDENTIFICATION_CODE;
                        if (request[i].IDENTIFICATION_CODE == 2) {
                            tr += "<td>共借人</td>";
                        } else {
                            tr += "<td>担保人</td>";
                        }
                        tr += "<td>" + sqr + "</td>";
                        tr += "<td>" + request[i].EXP_APPLICANT_CARD_ID + "</td>";
                        tr += "<td>" + request[i].EXP_APPLICANT_NAME + "</td>";
                        tr += "<td>" + request[i].BUSINESS_PARTNER_NME + "</td>";
                        tr += "<td>" + request[i].FP_GROUP_NME + "</td>";
                        tr += "<td>" + request[i].CONTRACT_DTE.split(" ")[0] + "</td>";
                        tr += "<td>" + request[i]['ROUND(CME.INTEREST_RATE,2)'] + "</td>";
                        tr += "<td>" + request[i]['ROUND(CME.NET_FINANCED_AMT)'] + "</td>";
                        tr += "<td>" + request[i].ASSET_MAKE_DSC + "</td>";
                        tr += "<td>" + request[i].ASSET_MODEL_DSC + "</td>";
                        tr += "<td>" + request[i].ASSET_BRAND_DSC + "</td>";
                        tr += "<td>" + request[i].OVERDUE_30_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_60_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_90_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_ABOVE_90_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_ABOVE_120_DAYS + "</td>";
                        tr += "<td>" + request[i].NO_OF_TERMS + "</td>";
                        tr += "<td>" + request[i].NO_OF_TERMS_PAID + "</td>";
                        tr += "</tr>";
                        $("#cmsApplication1").append(tr);    
                    }
                    $("#gjrcmsck").html(gjrcmsck);
                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function getCMSCKJE() {
            var fullckje = 0;
            var cmsckje = 0;
            var IdCardNo = $("#ctrlIdCardNo").val();
            var applicationno = $("#applicationNo").val();
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getCMSckje",// 19.6.28 wangxg
                data: { CommandName: "getCMSckje", applicantcardid: IdCardNo, applicationnumber: applicationno, cap: "cap" },
                type: "POST",
                dataType: "text",
                success: function (result) {
                    var request = eval('(' + result + ')');
                    console.log(request);
                    for (var i = 0; i < request.length; i++) {
                        var tr = "<tr>";
                        if (request[i].IS_EXPOSED_IND == "T") {
                            tr += "<td><input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/></td>";
                            cmsckje = cmsckje * 1 + request[i]['ROUND(CME.PRINCIPLE_OUTSTANDING_AMT)'] * 1;
                        } else {
                            tr += "<td><input disabled=\"disabled\" type=\"checkbox\"/></td>";
                        }
                        tr += "<td>" + request[i]['ROUND(CME.PRINCIPLE_OUTSTANDING_AMT)'] + "</td>";
                        tr += "<td><a href=\"javascript:void(0);\" onclick=\"getinstence('" + request[i].EXP_APPLICATION_NUMBER + "')\" >" + request[i].EXP_APPLICATION_NUMBER + "</a></td>";
                        tr += "<td>" + request[i].CONTRACT_NUMBER + "</td>";
                        tr += "<td>" + request[i].REQUEST_STATUS_DSC + "</td>";
                        var active = request[i].CONTRACT_STATUS_CDE == "20" ? "active" : request[i].CONTRACT_STATUS_CDE;
                        tr += "<td>" + active + "</td>";
                        var sqr = request[i].IDENTIFICATION_CODE == "1" ? "借款人" : request[i].IDENTIFICATION_CODE;
                        tr += "<td>" + sqr + "</td>";
                        tr += "<td>" + request[i].EXP_APPLICANT_CARD_ID + "</td>";
                        tr += "<td>" + request[i].EXP_APPLICANT_NAME + "</td>";
                        tr += "<td>" + request[i].BUSINESS_PARTNER_NME + "</td>";
                        tr += "<td>" + request[i].FP_GROUP_NME + "</td>";
                        tr += "<td>" + request[i].CONTRACT_DTE.split(" ")[0] + "</td>";
                        tr += "<td>" + request[i]['ROUND(CME.INTEREST_RATE,2)'] + "</td>";
                        tr += "<td>" + request[i]['ROUND(CME.NET_FINANCED_AMT)'] + "</td>";
                        tr += "<td>" + request[i].ASSET_MAKE_DSC + "</td>";
                        tr += "<td>" + request[i].ASSET_MODEL_DSC + "</td>";
                        tr += "<td>" + request[i].ASSET_BRAND_DSC + "</td>";
                        tr += "<td>" + request[i].OVERDUE_30_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_60_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_90_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_ABOVE_90_DAYS + "</td>";
                        tr += "<td>" + request[i].OVERDUE_ABOVE_120_DAYS + "</td>";
                        tr += "<td>" + request[i].NO_OF_TERMS + "</td>";
                        tr += "<td>" + request[i].NO_OF_TERMS_PAID + "</td>";
                        tr += "</tr>";
                        $("#cmsApplication").append(tr);
                    }
                    $("#cmsckje").html(cmsckje);
                    $("#fullckje").html(($("#cmsckje").html() * 1) + ($("#capckje").html() * 1));
                    $("#ctrlCkje").html(($("#cmsckje").html() * 1) + ($("#capckje").html() * 1));
                },
                //error: function (msg) {
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function getinstence(applicationno) {
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getinstenceid",// 19.6.28 wangxg
                data: { CommandName: "getinstenceid", applicationno: applicationno },
                //data: { CommandName: "getinstenceid", applicationno: 'Br-A060017000' },
                type: "POST",
                async: false,
                dataType: "json",
                success: function (result) {
                    if (result.Result == 'nofind') {
                        alert("未查询到此单");
                    }
                    else {
                        //$("#aclick").attr("href", "../../../InstanceSheets.html?InstanceId=" + result.Result);
                        $("#aclick").attr("href", "SRetailApp.aspx?SheetCode=SRetailApp&Mode=View&InstanceId="+ result.Result +"&SheetCode=SRetailApp");
                        document.getElementById("aclick").click();
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
        function capcheck(ts,ck) {
            console.log(ts.dataset.number);
            if (ts.checked) {
                $("#" + ck).html(($("#" + ck).html() * 1) + (ts.dataset.number * 1));
            } else {
                $("#" + ck).html(($("#" + ck).html() * 1) - (ts.dataset.number * 1));
            }
            $("#fullckje").html(($("#capckje").html() * 1) + ($("#cmsckje").html() * 1));
            $("#ctrlCkje").html(($("#cmsckje").html() * 1) + ($("#capckje").html() * 1));
        }
        //获取申请人PBOC人行报告
        function pbocClick(name, idtype, id) {
            var InstanceId = '<%=this.ActionContext.InstanceId%>';
            var operator = '<%=this.ActionContext.User.UserName%>';
            var pbocurl = '<%=ConfigurationManager.AppSettings["pbocurl"] + string.Empty%>';
            var name = $("#" + name).val().replace(/\"/g, "\\\"");
            var idtype = $("#" + idtype).val();
            var id = $("#" + id).val();
            if (!name || name == "" || !idtype || idtype == "" || !id || id == "") {
                alert("所查信息不存在");
                return false;
            }
            var datajson = "{\"reportRequest\":{\"repID\":\"" + InstanceId + "\",\"name\":\"" + name + "\",\"idtype\":\"" + idtype + "\",\"id\":\"" + id + "\",\"time\":\"\",\"operator\":\"" + operator + "\"}}";
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx",
                url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
                data:
                {
                    CommandName: "getRSResult",
                    param: datajson,
                    address: pbocurl
                },
                type: "post",
                async: false,
                dataType: "json",
                success: function (result) {
                    if (result.report && result.report != "") {
                        $("#aclick").attr("href", result.report);
                        document.getElementById("aclick").click();
                    }
                    else {
                        alert("融数反馈结果：" + result.msg);
                    }
                },
                //error: function (msg) {
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
            return false;
        }

        function rsfkClick() {
            var InstanceId = '<%=this.ActionContext.InstanceId%>';
            var Control29 = encodeURI($("#Control29").val());
            var Control122 = encodeURI($("#Control122").val());
            var Control209 = encodeURI($("#Control209").val());
            var rsfkurl = '<%=ConfigurationManager.AppSettings["rsfkurl"] + string.Empty%>';
            var url = "../view/rsfkresult.html?&rsfkurl=" + rsfkurl + "&InstanceId=" + InstanceId;
            $("#aclick").attr("href", url);
            document.getElementById("aclick").click();
            return false;
        }

        function nciicClick() {
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            var nciicurl = '<%=ConfigurationManager.AppSettings["nciicurl"] + string.Empty%>';
            var ncurl = "../view/NCIIC.html?&nciicurl=" + nciicurl + "&instanceid=" + instanceid;
            $("#aclick").attr("href", ncurl);
            document.getElementById("aclick").click();
            //var jsontxt = { CommandName: "getNCIIC", param: instanceid, url: nciicurl };
            //$.ajax({
            //    //url: "../../../ajax/RSHandler.ashx",
            //    url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
            //    data:
            //    {
            //        CommandName: "getNCIIC",
            //        param: "{\"reqID\": \"" + instanceid + "\" }",
            //        address: nciicurl
            //    },
            //    type: "post",
            //    async: true,
            //    dataType: "json",
            //    success: function (result) {

            //    },
            //    error: function (msg) {
            //        alert(msg.responseText + "出错了");
            //    }
            //});
            return false;
        }
        //获取当前时间
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var seperator2 = ":";
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                + " " + date.getHours() + seperator2 + date.getMinutes()
                + seperator2 + date.getSeconds();
            return currentdate;
        }
        //隐藏附加上传空白行
        function hideFIrow() {
            $("#divattachment .row").each(function () {
                if ($(this).find(">.col-md-2>span").css("display") == "none") {
                    $(this).hide();
                }
            })
        }
        //隐藏二级菜单
        function hidemsg() {
            //申请人
            $("#divSQR").find(".nav-icon")[1].click();
            $("#divSQR").find(".nav-icon")[2].click();
            $("#divSQR").find(".nav-icon")[3].click();
            //共借人
            $("#divGJR").find(".nav-icon")[1].click();
            $("#divGJR").find(".nav-icon")[2].click();
            $(".bannerTitle")[2].click();
            //担保人
            $("#divDBR").find(".nav-icon")[0].click();
            $("#divDBR").find(".nav-icon")[1].click();
            $("#divDBR").find(".nav-icon")[2].click();
            $(".bannerTitle")[3].click();
            //资产
            //金融
            //附件
            $(".bannerTitle")[6].click();
            //审核
            $(".bannerTitle")[7].click();
            hidepboc();
            //隐藏融数
            if ($("#Control380").val() == "overtime") {
                $("#divBascSQRInfo").find(">div:eq(0)").find("a").hide();
                $("#rsmanchk").show();
            }
        }
        function hidepboc(){
            //共借人不存在隐藏
            if ($("#Control124").val() == "") {
                $("#aclickh").find("a:eq(1)").hide();
                $("#Label6").parent().hide();
            }
            //担保人不存在隐藏
            if ($("#Control211").val() == "") {
                $("#aclickh").find("a:eq(2)").hide();
                $("#Label7").parent().hide();
            }
        }
        //人工查询
        function rsmanchk() {
            $("#rsmanchk").attr("disabled", "disabled");
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx?Math=" + Math.random(),
                url: "/Portal/RSHandler/Index?Math=" + Math.random(),// 19.6.28 wangxg
                data:
                {
                    CommandName: "postRongshu",
                    instanceid: instanceid,
                    SchemaCode: "RetailApp",
                    manual: "2"
                },
                type: "post",
                async: true,
                dataType: "json",
                success: function (result) {
                    //
                    $("#searching").html("查询中...");
                    timegets();
                },
                //error: function (msg) {
                //    $("#rsmanchk").attr("disabled", false);
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
                    $("#rsmanchk").attr("disabled", false);
					 showJqErr(msg);
                }
            });
        }
        function timegets() {
            var rsfkurl = '<%=ConfigurationManager.AppSettings["rsfkurl"] + string.Empty%>';
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx",
                url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
                data:
                {
                    CommandName: "getRSResult",
                    param: "{\"reqID\": \"" + instanceid + "\" }",
                    address: rsfkurl
                },
                type: "post",
                async: true,
                dataType: "json",
                success: function (result) {
                    debugger;
                    if (result.code== "00") {
                        $("#rsmanchk").attr("disabled", false);
                        $("#divBascSQRInfo").find(">div:eq(0)").find("a").show();
                        //$("#searching").hide();
                        $("#searching").html("");
                        //$("#rsmanchk").hide();
                        hidepboc();
                    } else {
                        setTimeout("timegets()", 10000);
                    }
                },
                //error: function (msg) {
                //    $("#rsmanchk").attr("disabled", false);
                //    alert(msg.responseText + "出错了");
                //},
                error: function (msg) {// 19.7 
                    $("#rsmanchk").attr("disabled", false);
					 showJqErr(msg);
                }
            });
        }
        //银行卡数字验证
        function textnum(ts) {
            if (!/^[0-9]*$/.test($(ts).val())) {
                $(ts).val("");
                alert("请输入数字!");
            }
        }
        // 页面加载完成事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            //隐藏批量下载
            if ($.MvcSheetUI.SheetInfo.ActivityCode != "FI1") {
                $(".SheetAttachment").find("a").hide();
            }
            getCkje();
            getmsg();
            hideFIrow();
            hidemsg();
            //添加留言
            $('#addmsga').on('click',function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
            var Activity = '<%=this.ActionContext.ActivityCode%>';
            if (Activity == "Activity2") {
                $("#main-navbar").hide();
            }        

        }
        // 保存事件
        $.MvcSheet.SaveAction.Save = function () {
            // this.Action  // 获取当前按钮名称
            alert($.MvcSheetUI.SheetInfo.InstanceId);
            $.MvcSheetUI.SetControlValue("CJH", $.MvcSheetUI.SheetInfo.InstanceId);
        }

        $.MvcSheet.SaveAction.OnActionDone = function () {
            // this.Action  // 获取当前按钮名称
            //alert($.MvcSheetUI.SheetInfo.InstanceId);
            //$.MvcSheetUI.SetControlValue("instanceid", $.MvcSheetUI.SheetInfo.InstanceId);
        }
        function gridAddRow(grid, args) {
            if (args[1]) {
                // args[1] 只会在页面加载的时候有值，添加的行是没有值的
                var code = args[1].DataItems["LY.LYRY"].V;
                alert(code);
            }
        }
    </script>
</asp:Content>
