<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HSCompanyApp.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.HSCompanyApp" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

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
    </style>
    <script type="text/javascript">

</script>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">机构汽车贷款申请</label>
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
                <div id="title20" class="col-md-2">
                    <span id="Label28" data-type="SheetLabel" data-datafield="IS_INACTIVE_IND" style="">是否隐藏共借人</span>
                </div>
                <div id="control20" class="col-md-4">
                    <input id="Control28" type="text" data-datafield="IS_INACTIVE_IND" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row tableContent">
                <div id="title17" class="col-md-2">
                    <span id="Label26" data-type="SheetLabel" data-datafield="dealcomments" style="">留言备注</span>
                </div>
                <div id="control17" class="col-md-10">
                    <textarea id="Control26" data-datafield="dealcomments" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>

        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divSQR',this)">
            <label id="Label5" data-en_us="Sheet information">申请信息</label>
        </div>
        <div class="divContent" id="divSQR">
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInfo',this)">
                <label id="Label8" data-en_us="Sheet information">基本信息</label>
            </div>
            <div id="divBascSQRInfo">
                <div class="row">
                    <div class="col-md-2">
                        <span data-type="SheetLabel" style="display:block;">风控模型评估</span>
                    </div>
                    <div class="col-md-2">
                        <span><a href="../view/NCIIC.html?instanceid=<%= this.ActionContext.InstanceId%>" target="_blank">评估结果</a></span>
                    </div>
                    <div class="col-md-2">
                        <span><a href="#" target="_blank">PBOC</a></span>
                    </div>
                    <div class="col-md-2">
                        <span><a target="_blank" href="#">NCIIC</a></span>
                    </div>
                </div>
                <div class="row">
                    <div id="title3461" class="col-md-2">
                        <span id="Label3091" data-type="SheetLabel" data-datafield="organizationCode" style="">组织机构代码</span>
                    </div>
                    <div id="control3461" class="col-md-4">
                        <input id="Control3091" type="text" data-datafield="organizationCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3471" class="col-md-2">
                        <span id="Label3101" data-type="SheetLabel" data-datafield="CregistrationNo" style="">注册号</span>
                    </div>
                    <div id="control3471" class="col-md-4">
                        <input id="Control3101" type="text" data-datafield="CregistrationNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title3481" class="col-md-2">
                        <span id="Label3111" data-type="SheetLabel" data-datafield="companyNamech" style="">公司名称(中文）</span>
                    </div>
                    <div id="control3481" class="col-md-4">
                        <input id="Control3111" type="text" data-datafield="companyNamech" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3491" class="col-md-2">
                        <span id="Label3121" data-type="SheetLabel" data-datafield="cbusinesstypeName" style="">企业类型</span>
                    </div>
                    <div id="control3491" class="col-md-4">
                        <input id="Control3121" type="text" data-datafield="cbusinesstypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title3501" class="col-md-2">
                        <span id="Label3131" data-type="SheetLabel" data-datafield="capitalReamount" style="">注册资本金额</span>
                    </div>
                    <div id="control3501" class="col-md-4">
                        <input id="Control3131" type="text" data-datafield="capitalReamount" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3511" class="col-md-2">
                        <span id="Label3141" data-type="SheetLabel" data-datafield="establishedin" style="">成立自</span>
                    </div>
                    <div id="control3511" class="col-md-4">
                        <input id="Control3141" type="text" data-datafield="establishedin" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title3521" class="col-md-2">
                        <span id="Label3151" data-type="SheetLabel" data-datafield="parentCompany" style="">母公司</span>
                    </div>
                    <div id="control3521" class="col-md-4">
                        <input id="Control3151" type="text" data-datafield="parentCompany" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3531" class="col-md-2">
                        <span id="Label3161" data-type="SheetLabel" data-datafield="subsidaryCompany" style="">子公司</span>
                    </div>
                    <div id="control3531" class="col-md-4">
                        <input id="Control3161" type="text" data-datafield="subsidaryCompany" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title3541" class="col-md-2">
                        <span id="Label3171" data-type="SheetLabel" data-datafield="cindustrytypeName" style="">行业类型</span>
                    </div>
                    <div id="control3541" class="col-md-4">
                        <input id="Control3171" type="text" data-datafield="cindustrytypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3551" class="col-md-2">
                        <span id="Label3181" data-type="SheetLabel" data-datafield="cindustrysubtypeName" style="">行业子类型</span>
                    </div>
                    <div id="control3551" class="col-md-4">
                        <input id="Control3181" type="text" data-datafield="cindustrysubtypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title3561" class="col-md-2">
                        <span id="Label3191" data-type="SheetLabel" data-datafield="representativeName" style="">法人姓名</span>
                    </div>
                    <div id="control3561" class="col-md-4">
                        <input id="Control3191" type="text" data-datafield="representativeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3571" class="col-md-2">
                        <span id="Label3201" data-type="SheetLabel" data-datafield="representativeidNo" style="">法人身份证件号码</span>
                    </div>
                    <div id="control3571" class="col-md-4">
                        <input id="Control3201" type="text" data-datafield="representativeidNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title3581" class="col-md-2">
                        <span id="Label3211" data-type="SheetLabel" data-datafield="representativeDesignation" style="">法人职务</span>
                    </div>
                    <div id="control3581" class="col-md-4">
                        <input id="Control3211" type="text" data-datafield="representativeDesignation" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3591" class="col-md-2">
                        <span id="Label3221" data-type="SheetLabel" data-datafield="loanCard" style="">贷款卡号</span>
                    </div>
                    <div id="control3591" class="col-md-4">
                        <input id="Control3221" type="text" data-datafield="loanCard" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title360" class="col-md-2">
                        <span id="Label3231" data-type="SheetLabel" data-datafield="loancardPassword" style="">贷款卡密码</span>
                    </div>
                    <div id="control360" class="col-md-4">
                        <input id="Control3231" type="text" data-datafield="loancardPassword" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3611" class="col-md-2">
                        <span id="Label3241" data-type="SheetLabel" data-datafield="compaynameeng" style="">公司名称(英文)</span>
                    </div>
                    <div id="control3611" class="col-md-4">
                        <input id="Control3241" type="text" data-datafield="compaynameeng" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title362" class="col-md-2">
                        <span id="Label3251" data-type="SheetLabel" data-datafield="taxid" style="">税号</span>
                    </div>
                    <div id="control362" class="col-md-4">
                        <input id="Control3251" type="text" data-datafield="taxid" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3631" class="col-md-2">
                        <span id="Label3261" data-type="SheetLabel" data-datafield="businessHistory" style="">公司年限</span>
                    </div>
                    <div id="control3631" class="col-md-4">
                        <input id="Control3261" type="text" data-datafield="businessHistory" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title364" class="col-md-2">
                        <span id="Label3271" data-type="SheetLabel" data-datafield="trustName" style="">信托机构名称</span>
                    </div>
                    <div id="control364" class="col-md-4">
                        <input id="Control3271" type="text" data-datafield="trustName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3651" class="col-md-2">
                        <span id="Label3281" data-type="SheetLabel" data-datafield="annualRevenue" style="">收入</span>
                    </div>
                    <div id="control3651" class="col-md-4">
                        <input id="Control3281" type="text" data-datafield="annualRevenue" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title366" class="col-md-2">
                        <span id="Label3291" data-type="SheetLabel" data-datafield="networthamt" style="">净资产额</span>
                    </div>
                    <div id="control366" class="col-md-4">
                        <input id="Control3291" type="text" data-datafield="networthamt" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3671" class="col-md-2">
                        <span id="Label3301" data-type="SheetLabel" data-datafield="years" style="">年份</span>
                    </div>
                    <div id="control3671" class="col-md-4">
                        <input id="Control3301" type="text" data-datafield="years" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title368" class="col-md-2">
                        <span id="Label3311" data-type="SheetLabel" data-datafield="cemailAddress" style="">邮箱地址</span>
                    </div>
                    <div id="control368" class="col-md-4">
                        <input id="Control3311" type="text" data-datafield="cemailAddress" data-type="SheetTextBox" style="">
                    </div>
                </div>


               
             
                <div class="row">
                    <div id="title63" class="col-md-2">
                        <span id="Label69" data-type="SheetLabel" data-datafield="lienee" style="">抵押人</span>
                    </div>
                    <div id="control63" class="col-md-4">
                        <input id="Control69" type="text" data-datafield="lienee" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title62" class="col-md-2">
                        <span id="Label68" data-type="SheetLabel" data-datafield="BlacklistInd" style="">外部信用记录</span>
                    </div>
                    <div id="control62" class="col-md-4">
                        <input id="Control68" type="text" data-datafield="BlacklistInd" data-type="SheetTextBox" style="">
                    </div>
                </div>
                   <div class="row">
                    <div id="title61" class="col-md-2">
                        <span id="Label67" data-type="SheetLabel" data-datafield="BlackListNoRecordInd" style="">黑名单无记录</span>
                    </div>
                    <div id="control61" class="col-md-4">
                        <input id="Control67" type="text" data-datafield="BlackListNoRecordInd" data-type="SheetTextBox" style="">
                    </div>
                    
                </div>
            </div>
            
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divsqrdz',this)">
                <label id="Label10" data-en_us="Sheet information">地址</label>
            </div>
            <div id="divsqrdz">
                <div class="row tableContent">
                    <div id="title83" class="col-md-2">
                        <span id="Label85" data-type="SheetLabel" data-datafield="currentLivingAddress" style="">地址</span>
                    </div>
                    <div id="control83" class="col-md-10">
                        <textarea id="Control85" data-datafield="currentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title85" class="col-md-2">
                        <span id="Label86" data-type="SheetLabel" data-datafield="AddressId" style="">注册地址</span>
                    </div>
                    <div id="control85" class="col-md-10">
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
                        <span id="Label88" data-type="SheetLabel" data-datafield="hukouaddress" style="">办公地址</span>
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
               

                    <div id="title92" class="col-md-2">
                        <span id="Label92" data-type="SheetLabel" data-datafield="hukouprovinceName" style="">省份</span>
                    </div>
                    <div id="control92" class="col-md-4">
                        <input id="Control92" type="text" data-datafield="hukouprovinceName" data-type="SheetTextBox" style="">
                    </div>
                   </div>
                     <div class="row">
                    <div id="title93" class="col-md-2">
                        <span id="Label93" data-type="SheetLabel" data-datafield="hukoucityName" style="">城市</span>
                    </div>
                    <div id="control93" class="col-md-4">
                        <input id="Control93" type="text" data-datafield="hukoucityName" data-type="SheetTextBox" style="">
                    </div>
                          <div id="title95" class="col-md-2">
                        <span id="Label95" data-type="SheetLabel" data-datafield="postcode" style="">邮编</span>
                    </div>
                    <div id="control95" class="col-md-4">
                        <input id="Control95" type="text" data-datafield="postcode" data-type="SheetTextBox" style="">
                    </div>
                </div>
               
                <div class="row">
                   
                    <div id="title96" class="col-md-2">
                        <span id="Label96" data-type="SheetLabel" data-datafield="addresstypeName" style="">地址类型</span>
                    </div>
                    <div id="control96" class="col-md-4">
                        <input id="Control96" type="text" data-datafield="addresstypeName" data-type="SheetTextBox" style="">
                    </div>
                
                    <div id="title97" class="col-md-2">
                        <span id="Label97" data-type="SheetLabel" data-datafield="addressstatusName" style="">地址状态</span>
                    </div>
                    <div id="control97" class="col-md-4">
                        <input id="Control97" type="text" data-datafield="addressstatusName" data-type="SheetTextBox" style="">
                    </div>
                    </div>
                <div class="row">
                    <div id="title98" class="col-md-2">
                        <span id="Label98" data-type="SheetLabel" data-datafield="propertytypeName" style="">房产类型</span>
                    </div>
                    <div id="control98" class="col-md-4">
                        <input id="Control98" type="text" data-datafield="propertytypeName" data-type="SheetTextBox" style="">
                    </div>
               
                    <div id="title99" class="col-md-2">
                        <span id="Label99" data-type="SheetLabel" data-datafield="residencetypeName" style="">住宅类型</span>
                    </div>
                    <div id="control99" class="col-md-4">
                        <input id="Control99" type="text" data-datafield="residencetypeName" data-type="SheetTextBox" style="">
                    </div>
                     </div>
                <div class="row">
                    <div id="title100" class="col-md-2">
                        <span id="Label100" data-type="SheetLabel" data-datafield="livingsince" style="">开始居住日期</span>
                    </div>
                    <div id="control100" class="col-md-4">
                        <input id="Control100" type="text" data-datafield="livingsince" data-type="SheetTextBox" style="">
                    </div>
              
                    <div id="title101" class="col-md-2">
                        <span id="Label101" data-type="SheetLabel" data-datafield="homevalue" style="">房屋价值（万元)</span>
                    </div>
                    <div id="control101" class="col-md-4">
                        <input id="Control101" type="text" data-datafield="homevalue" data-type="SheetTextBox" style="">
                    </div>
                      </div>
                <div class="row">
                    <div id="title102" class="col-md-2">
                        <span id="Label102" data-type="SheetLabel" data-datafield="stayinyear" style="">居住年限</span>
                    </div>
                    <div id="control102" class="col-md-4">
                        <input id="Control102" type="text" data-datafield="stayinyear" data-type="SheetTextBox" style="">
                    </div>
                
                    <div id="title103" class="col-md-2">
                        <span id="Label103" data-type="SheetLabel" data-datafield="countrycodeName" style="">国家代码</span>
                    </div>
                    <div id="control103" class="col-md-4">
                        <input id="Control103" type="text" data-datafield="countrycodeName" data-type="SheetTextBox" style="">
                    </div>
                    </div>
                <div class="row">
                    <div id="title104" class="col-md-2">
                        <span id="Label104" data-type="SheetLabel" data-datafield="areaCode" style="">地区代码</span>
                    </div>
                    <div id="control104" class="col-md-4">
                        <input id="Control104" type="text" data-datafield="areaCode" data-type="SheetTextBox" style="">
                    </div>
                
                    <div id="title105" class="col-md-2">
                        <span id="Label105" data-type="SheetLabel" data-datafield="phoneNo" style="">电话</span>
                    </div>
                    <div id="control105" class="col-md-4">
                        <input id="Control105" type="text" data-datafield="phoneNo" data-type="SheetTextBox" style="">
                    </div>
                    </div>
                <div class="row">
                    <div id="title106" class="col-md-2">
                        <span id="Label106" data-type="SheetLabel" data-datafield="extension" style="">分机</span>
                    </div>
                    <div id="control106" class="col-md-4">
                        <input id="Control106" type="text" data-datafield="extension" data-type="SheetTextBox" style="">
                    </div>
                
                    <div id="title107" class="col-md-2">
                        <span id="Label107" data-type="SheetLabel" data-datafield="phonetypeName" style="">电话类型</span>
                    </div>
                    <div id="control107" class="col-md-4">
                        <input id="Control107" type="text" data-datafield="phonetypeName" data-type="SheetTextBox" style="">
                    </div>
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
                <div class="row">
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
                </div>
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
                        <input id="Control392" type="text" data-datafield="HQRJ" data-type="SheetTextBox" style="" />
                    </div>
                    <div id="title438" class="col-md-2">
                        <span id="Label393" data-type="SheetLabel" data-datafield="JRZC" style="">金融资产</span>
                    </div>
                    <div id="control438" class="col-md-4">
                        <input id="Control393" type="text" data-datafield="JRZC" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title439" class="col-md-2">
                        <span id="Label394" data-type="SheetLabel" data-datafield="SRFZB" style="">收入负债比</span>
                    </div>
                    <div id="control439" class="col-md-4">
                        <input id="Control394" type="text" data-datafield="SRFZB" data-type="SheetTextBox" style="">
                    </div>
                    <div id="space440" class="col-md-2">
                    </div>
                    <div id="spaceControl440" class="col-md-4">
                    </div>
                </div>
            </div>
            
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
                    <div id="space222" class="col-md-2">
                    </div>
                    <div id="spaceControl222" class="col-md-4">
                    </div>
                </div>
                <div class="row">
                    <div id="title223" class="col-md-2">
                        <span id="Label214" data-type="SheetLabel" data-datafield="DbHokouName" style="">担保人户口所在地</span>
                    </div>
                    <div id="control223" class="col-md-10">
                        <textarea id="Control214" data-datafield="DbHokouName" data-type="SheetTextBox" style="">					</textarea>
                    </div>
                </div>
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
                        <span id="Label235" data-type="SheetLabel" data-datafield="DbEducationName" style="">担保人教育程度 </span>
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
                        <input id="Control250" data-datafield="DbCompanyName" data-type="SheetTextBox" style="">					</input>
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
    <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divDBR1',this)">
            <label id="Label711" data-en_us="Sheet information">担保公司信息</label>
        </div>
    <div class="divContent" id="divDBR1">

            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRIno61',this)">
                <label data-en_us="Sheet information">担保公司基本信息</label>
            </div>
            <div id="divBascSQRIno61">

                <div class="row">
                    <div id="title3692" class="col-md-2">
                        <span id="Label3322" data-type="SheetLabel" data-datafield="DbCorganizationCode" style="">担保公司组织机构代码</span>
                    </div>
                    <div id="control3692" class="col-md-4">
                        <input id="Control3322" type="text" data-datafield="DbCorganizationCode" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title370" class="col-md-2">
                        <span id="Label3332" data-type="SheetLabel" data-datafield="DbCregistrationNo" style="">担保公司注册号</span>
                    </div>
                    <div id="control370" class="col-md-4">
                        <input id="Control3332" type="text" data-datafield="DbCregistrationNo" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3712" class="col-md-2">
                        <span id="Label3342" data-type="SheetLabel" data-datafield="DbCcompanyNamech" style="">担保公司公司名称(中文）</span>
                    </div>
                    <div id="control3712" class="col-md-4">
                        <input id="Control3342" type="text" data-datafield="DbCcompanyNamech" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title372" class="col-md-2">
                        <span id="Label3352" data-type="SheetLabel" data-datafield="DbCbusinesstypeName" style="">担保公司企业类型</span>
                    </div>
                    <div id="control372" class="col-md-4">
                        <input id="Control3352" type="text" data-datafield="DbCbusinesstypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3732" class="col-md-2">
                        <span id="Label3362" data-type="SheetLabel" data-datafield="DbCcapitalReamount" style="">担保公司注册资本金额</span>
                    </div>
                    <div id="control3732" class="col-md-4">
                        <input id="Control3362" type="text" data-datafield="DbCcapitalReamount" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title374" class="col-md-2">
                        <span id="Label3372" data-type="SheetLabel" data-datafield="DbCestablishedin" style="">担保公司成立自</span>
                    </div>
                    <div id="control374" class="col-md-4">
                        <input id="Control3372" type="text" data-datafield="DbCestablishedin" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3752" class="col-md-2">
                        <span id="Label3382" data-type="SheetLabel" data-datafield="DbCparentCompany" style="">担保公司母公司</span>
                    </div>
                    <div id="control3752" class="col-md-4">
                        <input id="Control3382" type="text" data-datafield="DbCparentCompany" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title376" class="col-md-2">
                        <span id="Label33922" data-type="SheetLabel" data-datafield="DbCsubsidaryCompany" style="">担保公司子公司</span>
                    </div>
                    <div id="control376" class="col-md-4">
                        <input id="Control3392" type="text" data-datafield="DbCsubsidaryCompany" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title377" class="col-md-2">
                        <span id="Label3402" data-type="SheetLabel" data-datafield="DbCindustrytypeName" style="">担保公司行业类型</span>
                    </div>
                    <div id="control377" class="col-md-4">
                        <input id="Control3402" type="text" data-datafield="DbCindustrytypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title378" class="col-md-2">
                        <span id="Label3412" data-type="SheetLabel" data-datafield="DbCindustrysubtypeName" style="">担保公司行业子类型</span>
                    </div>
                    <div id="control378" class="col-md-4">
                        <input id="Control3412" type="text" data-datafield="DbCindustrysubtypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title379" class="col-md-2">
                        <span id="Label3422" data-type="SheetLabel" data-datafield="DbCrepresentativeName" style="">担保公司法人姓名</span>
                    </div>
                    <div id="control379" class="col-md-4">
                        <input id="Control34222" type="text" data-datafield="DbCrepresentativeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title380" class="col-md-2">
                        <span id="Label3432" data-type="SheetLabel" data-datafield="DbCrepresentativeidNo" style="">担保公司法人身份证件号码</span>
                    </div>
                    <div id="control380" class="col-md-4">
                        <input id="Control3432" type="text" data-datafield="DbCrepresentativeidNo" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title381" class="col-md-2">
                        <span id="Label3442" data-type="SheetLabel" data-datafield="DbCrepresentativeDesignation" style="">担保公司法人职务</span>
                    </div>
                    <div id="control381" class="col-md-4">
                        <input id="Control3442" type="text" data-datafield="DbCrepresentativeDesignation" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title382" class="col-md-2">
                        <span id="Label3452" data-type="SheetLabel" data-datafield="DbCloanCard" style="">担保公司贷款卡号</span>
                    </div>
                    <div id="control382" class="col-md-4">
                        <input id="Control3452" type="text" data-datafield="DbCloanCard" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title383" class="col-md-2">
                        <span id="Label3462" data-type="SheetLabel" data-datafield="DbCloancardPassword" style="">担保公司贷款卡密码</span>
                    </div>
                    <div id="control383" class="col-md-4">
                        <input id="Control3462" type="text" data-datafield="DbCloancardPassword" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title384" class="col-md-2">
                        <span id="Label3472" data-type="SheetLabel" data-datafield="DbCcomments" style="">担保公司评论</span>
                    </div>
                    <div id="control384" class="col-md-4">
                        <input id="Control3472" type="text" data-datafield="DbCcomments" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRIno51',this)">
                <label data-en_us="Sheet information">担保公司工作信息</label>
            </div>
            <div id="divBascSQRIno51">

                <div class="row">
                    <div id="title385" class="col-md-2">
                        <span id="Label3482" data-type="SheetLabel" data-datafield="DbCcompaynameeng" style="">担保公司公司名称(英文)</span>
                    </div>
                    <div id="control385" class="col-md-4">
                        <input id="Control3482" type="text" data-datafield="DbCcompaynameeng" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title386" class="col-md-2">
                        <span id="Label3492" data-type="SheetLabel" data-datafield="DbCtaxid" style="">担保公司税号</span>
                    </div>
                    <div id="control386" class="col-md-4">
                        <input id="Control3492" type="text" data-datafield="DbCtaxid" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title387" class="col-md-2">
                        <span id="Label350" data-type="SheetLabel" data-datafield="DbCbusinessHistory" style="">担保公司公司年限</span>
                    </div>
                    <div id="control387" class="col-md-4">
                        <input id="Control350" type="text" data-datafield="DbCbusinessHistory" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title388" class="col-md-2">
                        <span id="Label351" data-type="SheetLabel" data-datafield="DbCtrustName" style="">担保公司信托机构名称</span>
                    </div>
                    <div id="control388" class="col-md-4">
                        <input id="Control351" type="text" data-datafield="DbCtrustName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title389" class="col-md-2">
                        <span id="Label352" data-type="SheetLabel" data-datafield="DbCannualRevenue" style="">担保公司收入</span>
                    </div>
                    <div id="control389" class="col-md-4">
                        <input id="Control352" type="text" data-datafield="DbCannualRevenue" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title390" class="col-md-2">
                        <span id="Label353" data-type="SheetLabel" data-datafield="DbCnetworthamt" style="">担保公司净资产额</span>
                    </div>
                    <div id="control390" class="col-md-4">
                        <input id="Control353" type="text" data-datafield="DbCnetworthamt" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title3911" class="col-md-2">
                        <span id="Label354" data-type="SheetLabel" data-datafield="DbCyears" style="">担保公司年份</span>
                    </div>
                    <div id="control3911" class="col-md-4">
                        <input id="Control354" type="text" data-datafield="DbCyears" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title392" class="col-md-2">
                        <span id="Label355" data-type="SheetLabel" data-datafield="DbCemailAddress" style="">担保公司邮箱地址</span>
                    </div>
                    <div id="control392" class="col-md-4">
                        <input id="Control355" type="text" data-datafield="DbCemailAddress" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title393" class="col-md-2">
                        <span id="Label356" data-type="SheetLabel" data-datafield="DbClienee" style="">担保公司抵押人</span>
                    </div>
                    <div id="control393" class="col-md-4">
                        <input id="Control356" type="text" data-datafield="DbClienee" data-type="SheetTextBox" style="">
                    </div>
                    <div id="space394" class="col-md-2">
                    </div>
                    <div id="spaceControl394" class="col-md-4">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 100%;" onclick="hidediv('divBascSQRInf71',this)">
                <label data-en_us="Sheet information">担保公司地址联系方式</label>
            </div>
            <div id="divBascSQRInf71">

                <div class="row tableContent">
                    <div id="title395" class="col-md-2">
                        <span id="Label3571" data-type="SheetLabel" data-datafield="DbCcurrentLivingAddress" style="">担保公司地址</span>
                    </div>
                    <div id="control395" class="col-md-10">
                        <textarea id="Control3571" data-datafield="DbCcurrentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title397" class="col-md-2">
                        <span id="Label358" data-type="SheetLabel" data-datafield="DbCAddressId" style="">担保公司注册地址</span>
                    </div>
                    <div id="control397" class="col-md-10">
                        <textarea id="Control358" data-datafield="DbCAddressId" data-type="SheetRichTextBox" style="">					</textarea>
                    </div>
                </div>
                <div class="row">
                    <div id="title3991" class="col-md-2">
                        <span id="Label359" data-type="SheetLabel" data-datafield="DbCdefaultmailingaddress" style="">担保公司默认邮寄地址</span>
                    </div>
                    <div id="control3991" class="col-md-4">
                        <input id="Control359" type="text" data-datafield="DbCdefaultmailingaddress" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title400" class="col-md-2">
                        <span id="Label360" data-type="SheetLabel" data-datafield="DbChukouaddress" style="">担保公司户籍地址</span>
                    </div>
                    <div id="control400" class="col-md-4">
                        <input id="Control360" type="text" data-datafield="DbChukouaddress" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title4011" class="col-md-2">
                        <span id="Label3611" data-type="SheetLabel" data-datafield="DbCcountryName" style="">担保公司国家</span>
                    </div>
                    <div id="control4011" class="col-md-4">
                        <input id="Control3611" type="text" data-datafield="DbCcountryName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title402" class="col-md-2">
                        <span id="Label3621" data-type="SheetLabel" data-datafield="DbCprovinceName" style="">担保公司省份</span>
                    </div>
                    <div id="control402" class="col-md-4">
                        <input id="Control3621" type="text" data-datafield="DbCprovinceName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title403" class="col-md-2">
                        <span id="Label363" data-type="SheetLabel" data-datafield="DbCpostcode" style="">担保公司邮编</span>
                    </div>
                    <div id="control403" class="col-md-4">
                        <input id="Control363" type="text" data-datafield="DbCpostcode" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title404" class="col-md-2">
                        <span id="Label364" data-type="SheetLabel" data-datafield="DbCcityname" style="">担保公司城市</span>
                    </div>
                    <div id="control404" class="col-md-4">
                        <input id="Control364" type="text" data-datafield="DbCcityname" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title405" class="col-md-2">
                        <span id="Label365" data-type="SheetLabel" data-datafield="DbCaddresstypeName" style="">担保公司地址类型</span>
                    </div>
                    <div id="control405" class="col-md-4">
                        <input id="Control365" type="text" data-datafield="DbCaddresstypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title406" class="col-md-2">
                        <span id="Label366" data-type="SheetLabel" data-datafield="DbCaddressstatusName" style="">担保公司地址状态</span>
                    </div>
                    <div id="control406" class="col-md-4">
                        <input id="Control366" type="text" data-datafield="DbCaddressstatusName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title407" class="col-md-2">
                        <span id="Label367" data-type="SheetLabel" data-datafield="DbCpropertytypeName" style="">担保公司房产类型</span>
                    </div>
                    <div id="control407" class="col-md-4">
                        <input id="Control367" type="text" data-datafield="DbCpropertytypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title408" class="col-md-2">
                        <span id="Label368" data-type="SheetLabel" data-datafield="DbCresidencetypeName" style="">担保公司住宅类型</span>
                    </div>
                    <div id="control408" class="col-md-4">
                        <input id="Control368" type="text" data-datafield="DbCresidencetypeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title409" class="col-md-2">
                        <span id="Label369" data-type="SheetLabel" data-datafield="DbClivingsince" style="">担保公司开始居住日期</span>
                    </div>
                    <div id="control409" class="col-md-4">
                        <input id="Control369" type="text" data-datafield="DbClivingsince" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title410" class="col-md-2">
                        <span id="Label370" data-type="SheetLabel" data-datafield="DbChomevalue" style="">担保公司房屋价值（万元) </span>
                    </div>
                    <div id="control410" class="col-md-4">
                        <input id="Control370" type="text" data-datafield="DbChomevalue" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title411" class="col-md-2">
                        <span id="Label371" data-type="SheetLabel" data-datafield="DbCstayinyear" style="">担保公司居住年限</span>
                    </div>
                    <div id="control411" class="col-md-4">
                        <input id="Control371" type="text" data-datafield="DbCstayinyear" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title412" class="col-md-2">
                        <span id="Label372" data-type="SheetLabel" data-datafield="DbCareaCode" style="">担保公司地区代码</span>
                    </div>
                    <div id="control412" class="col-md-4">
                        <input id="Control372" type="text" data-datafield="DbCareaCode" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title413" class="col-md-2">
                        <span id="Label373" data-type="SheetLabel" data-datafield="DbCphoneNo" style="">担保公司电话</span>
                    </div>
                    <div id="control413" class="col-md-4">
                        <input id="Control373" type="text" data-datafield="DbCphoneNo" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title414" class="col-md-2">
                        <span id="Label374" data-type="SheetLabel" data-datafield="DbCextension" style="">担保公司分机</span>
                    </div>
                    <div id="control414" class="col-md-4">
                        <input id="Control374" type="text" data-datafield="DbCextension" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title415" class="col-md-2">
                        <span id="Label375" data-type="SheetLabel" data-datafield="DbCphonetypeName" style="">担保公司电话类型</span>
                    </div>
                    <div id="control415" class="col-md-4">
                        <input id="Control375" type="text" data-datafield="DbCphonetypeName" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title416" class="col-md-2">
                        <span id="Label376" data-type="SheetLabel" data-datafield="DbCcountrycodeName" style="">担保公司国家代码</span>
                    </div>
                    <div id="control416" class="col-md-4">
                        <input id="Control376" type="text" data-datafield="DbCcountrycodeName" data-type="SheetTextBox" style="">
                    </div>
                </div>
            </div>
        </div>
    <div class="nav-icon fa fa-chevron-down bannerTitle" onclick="hidediv('divZC',this)">
        <label id="Label3" data-en_us="Basic information">资产信息</label>
    </div>
    <div class="divContent" id="divZC">
        <div class="row">
            <div id="title304" class="col-md-2">
                <span id="Label288" data-type="SheetLabel" data-datafield="assetConditionName" style="">资产状况</span>
            </div>
            <div id="control304" class="col-md-4">
                <input id="Control288" type="text" data-datafield="assetConditionName" data-type="SheetTextBox" style="">
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
            <div id="title309" class="col-md-2">
                <span id="Label293" data-type="SheetLabel" data-datafield="assetPrice" style="">资产价格</span>
            </div>
            <div id="control309" class="col-md-4">
                <input id="Control293" type="text" data-datafield="assetPrice" data-type="SheetTextBox" style="">
            </div>
            <div id="title310" class="col-md-2">
                <span id="Label294" data-type="SheetLabel" data-datafield="transmissionName" style="">变速器</span>
            </div>
            <div id="control310" class="col-md-4">
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
            <div id="title312" class="col-md-2">
                <span id="Label296" data-type="SheetLabel" data-datafield="purposeName" style="">购车目的</span>
            </div>
            <div id="control312" class="col-md-4">
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
            <div id="title319" class="col-md-2">
                <span id="Label303" data-type="SheetLabel" data-datafield="vehicleAge" style="">车辆使用年数</span>
            </div>
            <div id="control319" class="col-md-4">
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
            <div id="title321" class="col-md-2">
                <span id="Label305" data-type="SheetLabel" data-datafield="releaseDate" style="">出厂日期</span>
            </div>
            <div id="control321" class="col-md-4">
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
            <div id="title329" class="col-md-2">
                <span id="Label313" data-type="SheetLabel" data-datafield="odometer" style="">里程数</span>
            </div>
            <div id="control329" class="col-md-4">
                <input id="Control313" type="text" data-datafield="odometer" data-type="SheetTextBox" style="">
            </div>
            <div id="title330" class="col-md-2">
                <span id="Label314" data-type="SheetLabel" data-datafield="totalaccessoryamount" style="">轮宽（改为附加费）</span>
            </div>
            <div id="control330" class="col-md-4">
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
            <div id="title337" class="col-md-2">
                <span id="Label319" data-type="SheetLabel" data-datafield="paymentFrequencyName" style="">付款频率</span>
            </div>
            <div id="control337" class="col-md-4">
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
            <div id="title339" class="col-md-2">
                <span id="Label321" data-type="SheetLabel" data-datafield="termMonth" style="">贷款期数（月）</span>
            </div>
            <div id="control339" class="col-md-4">
                <input id="Control321" type="text" data-datafield="termMonth" data-type="SheetTextBox" style="">
            </div>
            <div id="title340" class="col-md-2">
                <span id="Label322" data-type="SheetLabel" data-datafield="salesprice" style="">合同价格</span>
            </div>
            <div id="control340" class="col-md-4">
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
        <div class="row tableContent">
            <div id="title359" class="col-md-2">
                <span id="Label341" data-type="SheetLabel" data-datafield="hCompanyDetailtable" style="">还款计划</span>
            </div>
            <div id="control359" class="col-md-10">
                <table id="Control341" data-datafield="hCompanyDetailtable" data-type="SheetGridView" class="SheetGridView">
                    <tbody>

                        <tr class="header">
                            <td id="Control341_SerialNo" class="rowSerialNo">序号								</td>
                            <td id="Control341_Header3" data-datafield="hCompanyDetailtable.startTerm">
                                <label id="Control341_Label3" data-datafield="hCompanyDetailtable.startTerm" data-type="SheetLabel" style="">还款起始期</label>
                            </td>
                            <td id="Control341_Header4" data-datafield="hCompanyDetailtable.endTerm">
                                <label id="Control341_Label4" data-datafield="hCompanyDetailtable.endTerm" data-type="SheetLabel" style="">还款结束期</label>
                            </td>
                            <td id="Control341_Header5" data-datafield="hCompanyDetailtable.rentalAmount">
                                <label id="Control341_Label5" data-datafield="hCompanyDetailtable.rentalAmount" data-type="SheetLabel" style="">还款额</label>
                            </td>
                            <td id="Control341_Header6" data-datafield="hCompanyDetailtable.grossRentalAmount">
                                <label id="Control341_Label6" data-datafield="hCompanyDetailtable.grossRentalAmount" data-type="SheetLabel" style="">每期还款总额</label>
                            </td>
                            <td class="rowOption">删除								</td>
                        </tr>
                        <tr class="template">
                            <td id="Control341_Option" class="rowOption"></td>
                            <td data-datafield="hCompanyDetailtable.startTerm">
                                <input id="Control341_ctl3" type="text" data-datafield="hCompanyDetailtable.startTerm" data-type="SheetTextBox" style="">
                            </td>
                            <td data-datafield="hCompanyDetailtable.endTerm">
                                <input id="Control341_ctl4" type="text" data-datafield="hCompanyDetailtable.endTerm" data-type="SheetTextBox" style="">
                            </td>
                            <td data-datafield="hCompanyDetailtable.rentalAmount">
                                <input id="Control341_ctl5" type="text" data-datafield="hCompanyDetailtable.rentalAmount" data-type="SheetTextBox" style="">
                            </td>
                            <td data-datafield="hCompanyDetailtable.grossRentalAmount">
                                <input id="Control341_ctl6" type="text" data-datafield="hCompanyDetailtable.grossRentalAmount" data-type="SheetTextBox" style="">
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
                            <td data-datafield="hCompanyDetailtable.startTerm"></td>
                            <td data-datafield="hCompanyDetailtable.endTerm"></td>
                            <td data-datafield="hCompanyDetailtable.rentalAmount"></td>
                            <td data-datafield="hCompanyDetailtable.grossRentalAmount"></td>
                            <td class="rowOption"></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divattachment',this)">
        <label id="Label1" data-en_us="Sheet information">附件信息</label>
    </div>
    <div class="" id="divattachment">
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
            <div id="title363" class="col-md-2">
                <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
            </div>
            <div id="control363" class="col-md-10">
                <div id="Control343" data-datafield="SFZ" data-type="SheetAttachment" style="">
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
            <div id="title367" class="col-md-2">
                <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
            </div>
            <div id="control367" class="col-md-10">
                <div id="Control345" data-datafield="JSZ" data-type="SheetAttachment" style="">
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
            <div id="title371" class="col-md-2">
                <span id="Label347" data-type="SheetLabel" data-datafield="SR" style="">收入类材料</span>
            </div>
            <div id="control371" class="col-md-10">
                <div id="Control347" data-datafield="SR" data-type="SheetAttachment" style="">
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
            <div id="title375" class="col-md-2">
                <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
            </div>
            <div id="control375" class="col-md-10">
                <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" style="">
                </div>
            </div>
        </div>

        <div class="row">
            <div id="title391" class="col-md-2">
                <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">客户银行卡面复印件</span>
            </div>
            <div id="control391" class="col-md-10">
                <div id="Control357" data-datafield="YHKMFYJ" data-type="SheetAttachment" style="">
                </div>
            </div>
        </div>
    </div>
    
    <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
        <label id="Label2" data-en_us="Sheet information">审核信息</label>
    </div>
    <div class="divContent" id="div2">
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
        <div class="row tableContent">
            <div id="title423" class="col-md-2">
                <span id="Label381" data-type="SheetLabel" data-datafield="LYB" style="">留言板</span>
            </div>
            <div id="control423" class="col-md-10">
                <textarea id="Control381" data-datafield="LYB" data-type="SheetRichTextBox" style="">					</textarea>
            </div>
        </div>
        <div class="row tableContent">
            <div id="title425" class="col-md-2">
                <span id="Label382" data-type="SheetLabel" data-datafield="THJL" style="">通话记录</span>
            </div>
            <div id="control425" class="col-md-10">
                <textarea id="Control382" data-datafield="THJL" data-type="SheetRichTextBox" style="">					</textarea>
            </div>
        </div>
        <div class="row tableContent">
            <div id="title427" class="col-md-2">
                <span id="Label383" data-type="SheetLabel" data-datafield="NBLYB" style="">内部留言板</span>
            </div>
            <div id="control427" class="col-md-10">
                <textarea id="Control383" data-datafield="NBLYB" data-type="SheetRichTextBox" style="">					</textarea>
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


                $(ctrl1).css("margin-bottom", "8px");
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
                $(ctrl1).css("margin-bottom", "0px");

            }
        }
        $(function () {
            //getDownLoadURL();
            //viewer = new Viewer(document.getElementById('ulfjll'), { url: 'data-original' });
        })
        function getDownLoadURL() {
            var arryUrl = $(".SheetAttachment").find(".fa-download");
            $.each(arryUrl, function (i, item) {
                var name = $($($(item).parent().parent()).find("td")[0]).text();
                var type = /\.[^\.]+$/.exec(name)[0].toLowerCase();
                if (type == ".jpg" || type == ".gif" || type == ".png" || type == ".bmp" || type == ".jpeg") {
                    $("#ulfjll").append("<li><img data- original=\"" + item.href + "\" src= \"" + item.href + "\" alt= \"" + $(item).parent().parent().parent().parent().parent().parent().attr("data-title") + "\" ></li>");
                }
            })
            //隐藏批量下载
            if ($.MvcSheetUI.SheetInfo.ActivityCode != "FI1") {
                $(".SheetAttachment").find("a").hide();
            }

            viewer = new Viewer(document.getElementById('ulfjll'), { url: 'data-original' });


        }
        var viewer;
        // 页面加载完成事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            getDownLoadURL();
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

        $.MvcSheet.SaveAction.OnActionDone = function () {
            // this.Action  // 获取当前按钮名称
            //alert($.MvcSheetUI.SheetInfo.InstanceId);
            //$.MvcSheetUI.SetControlValue("instanceid", $.MvcSheetUI.SheetInfo.InstanceId);
        }
    </script>
</asp:Content>
