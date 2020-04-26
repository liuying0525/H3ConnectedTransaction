<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SRetailAppTEST.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SRetailAppTEST" EnableEventValidation="false" MasterPageFile="~/MvcSheetTEST.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="../../../jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <script src="../../../jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
    <link href="../../../WFRes/css/MvcSheetTest.css" rel="stylesheet" />
    <script src="../../../Custom/js/acl.js" type="text/javascript"></script>
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
        }

        body {
            font-size: 13px;
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

        .leftbox {
            width: 35%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        .leftbox2 {
            width: 35%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        .leftbox3 {
            width: 50%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        .leftbox::after ，.rightbox::after, .leftbox1::after, .rightbox1::after {
            display: block;
            clear: both;
        }

        .rightbox {
            width: 65%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        .rightbox2 {
            width: 65%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        .rightbox3 {
            width: 50%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

            .leftbox span, .leftbox label, .rightbox textarea, .rightbox div,
            .rightbox span, .rightbox label, .leftbox1 span, .leftbox1 label,
            .rightbox1 textarea, .rightbox1 span, .rightbox1 label, .rightbox1 div,
            .leftbox2 span, .leftbox2 label, .rightbox2 textarea, .rightbox2 div,
            .rightbox2 span, .rightbox2 label, .leftbox3 span, .leftbox3 label,
            .rightbox3 textarea, .rightbox3 div, .rightbox3 span, .rightbox3 label {
                padding-left: 0!important;
                padding-right: 0!important;
                padding-top: 0!important;
                padding-bottom: 0!important;
                margin-left: 0!important;
                margin-right: 0!important;
                margin-top: 0!important;
                margin-bottom: 0!important;
            }

        .leftbox1 {
            width: 17.5%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        .rightbox1 .comment > div {
            padding-left: 50px!important;
        }

        .rightbox1 {
            width: 82.5%;
            float: left;
            line-height: 20px;
            padding: 2px;
            font-size: 13px;
            text-align: center;
        }

        #div2 .rightbox1 {
            text-align: left;
        }

        .rightbox1 label div {
            overflow-x: inherit!important;
        }

        .isshowdetail {
            position: absolute;
            right: 0;
            bottom: 0px;
            width: 10%;
            height: 35px;
            line-height: 35px;
            text-align: center;
        }

        .centerline {
            width: 1px;
            padding-bottom: 10000px;
            margin-bottom: -10000px;
            position: relative;
            left: 35%;
            background: #ccc;
            top: -100px;
        }

        .centerline2 {
            width: 1px;
            padding-bottom: 10000px;
            margin-bottom: -10000px;
            position: relative;
            left: 35%;
            background: #ccc;
            top: -100px;
        }

        .centerline3 {
            width: 1px;
            padding-bottom: 10000px;
            margin-bottom: -10000px;
            position: relative;
            left: 50%;
            background: #ccc;
            top: -100px;
        }

        .fengkong {
            border: 1px solid #A2A2A2;
            font-size: 14px;
        }

            .fengkong > div {
                border: none;
            }

        #fktop {
            padding-left: 18px;
            padding-right: 18px;
            background: white;
            z-index: 99;
        }

        #div2 {
            max-height: 300px;
            overflow-y: scroll;
        }

        #fktop label {
            margin-bottom: 0;
        }

        #csjjxz .select2-chosen, #csjjxz .select2-arrow {
            margin: 7px 0!important;
        }

        @media screen and (max-width: 1024px) {
            #fktop {
                padding-left: 0;
                padding-right: 0;
            }
        }
    </style>
    <div class="panel-body sheetContainer" style="padding: 0;">
        <div id="fktop">
            <div class="row fengkong">
                <div id="wbsjy" style="width: 50%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a href="javascript:void(0);" onclick="rsfkClick();">风控评估模型评估结果</a></label>
                    <label><span id="fkpgjg" style="padding-left: 5px; padding-right: 5px;"></span></label>
                    <label><span style="padding-left: 5px;">客户还款测算:</span></label>
                    <label><span id="srfzc" style="padding-left: 5px;"></span></label>
                    <label style="width:50%;">
                        <span>信用评分:</span>
                        <span id="grxypf" style="padding-left: 5px;"></span>
                    </label>
                </div>
                <div style="width: 5%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a href="javascript:void(0);" onclick="nciicClick();">NCIIC</a></label>
                </div>
                <div style="width: 20%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;" id="aclickh">
                    <label>PBOC</label>
                    <a href="javascript:void(0);" onclick="pbocClick('Control29','Control30','ctrlIdCardNo');">申请人</a>
                    <a href="javascript:void(0);" onclick="pbocClick('Control122','Control123','Control124');">共借人</a>
                    <a href="javascript:void(0);" onclick="pbocClick('Control209','Control210','Control211');">担保人</a>
                </div>
                <div style="width: 15%; line-height: 20px; padding: 5px 0px 5px 0px; float: left; position: relative;">
                    <a class="btn" id="rsmanchk" onclick="rsmanchk()">人工查询</a>
                    <span id="searching" style="position: absolute; white-space: nowrap;"></span>
                </div>
                <div style="width: 10%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a href="javascript:void(0);" onclick="getDownLoadURL()">查看附件资料</a></label>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
                <label id="Label2" data-en_us="Sheet information">审核信息</label>
            </div>
            <div class="divContent" id="div2">
                <div class="row tableContent">
                    <div id="title427" class="leftbox1">
                        <span id="Label383" data-type="SheetLabel" data-datafield="NBLYB" style="">内部留言板</span>
                    </div>
                    <div id="control427" class="rightbox1">
                        <div class="leftbox1" style="width: 76%;">
                            <textarea id="Control383" data-datafield="NBLYB" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                        <div id="control467" class="leftbox1" style="width: 8%; position: relative; left: 8%; top: 10px;">
                            <input id="Control418" type="checkbox" data-datafield="ccbh" data-type="SheetCheckbox" style="">
                        </div>
                        <div id="title467" class="leftbox1" style="width: 16%; text-align: left; line-height: 40px;">
                            <span id="Label418" data-type="SheetLabel" data-datafield="ccbh" style="">差错驳回</span>
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
                <div class="row tableContent">
                    <div id="title399" class="leftbox1">
                        <span id="Label361" data-type="SheetLabel" data-datafield="CSYJ" style="">初审意见</span>
                    </div>
                    <div id="control399" class="rightbox1">
                        <div data-datafield="csshzt" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultselected="false" data-defaultitems="核准;拒绝;有条件核准;取消"></div>
                        <div id="csjjxz">
                            <div id="control493" class="rightbox" style="width: 50%; padding: 5px 0!important;">
                                <select data-datafield="xsjjlyzx" data-type="SheetDropDownList" id="ctl974208" class="" style=""
                                    data-schemacode="shyj1" data-querycode="shyj" data-displayemptyitem="true" data-filter="csshzt:TYPE2,个人零售:TYPE1,0:PARENTNAME"
                                    data-datavaluefield="REJECTNAME" data-datatextfield="REJECTNAME">
                                </select>
                            </div>
                            <div id="control494" class="rightbox" style="width: 50%; padding: 5px 0!important;">
                                <select data-datafield="xsjjlyzx1" data-type="SheetDropDownList" id="ctl625601" class=""
                                    style="" data-schemacode="shyj1" data-querycode="shyj" data-displayemptyitem="true"
                                    data-filter="csshzt:TYPE2,个人零售:TYPE1,xsjjlyzx:PARENTNAME"
                                    data-datavaluefield="REJECTNAME" data-datatextfield="REJECTNAME">
                                </select>
                            </div>
                        </div>
                        <div>
                            <div id="xscsyj" data-datafield="CSYJ" data-type="SheetComment" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row tableContent" id="xszsyj" style="position: relative; z-index: 999;">
                        <div id="title401" class="leftbox1">
                            <span id="Label362" data-type="SheetLabel" data-datafield="ZSYJj" style="">终审意见</span>
                        </div>
                        <div id="control401" class="rightbox1">
                            <div data-datafield="zsshzt" data-type="SheetRadioButtonList" id="ctl271485" class="" style="" data-defaultitems="核准;拒绝;有条件核准;取消"></div>
                            <div id="Control362" data-datafield="ZSYJj" data-type="SheetComment" style="">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label data-en_us="Basic information">经销商基本信息</label>
            <div style="float: right; font-weight: 300; font-size: 14px; padding-right: 5%; cursor: pointer;" onclick="showliuyan()">
                <img src="../../../img/Images/liuyan.png" style="position: relative; top: -4px;" />
                <span>留言</span>
                <a id="lysq">收起 &and;</a>
            </div>
        </div>
        <div class="divContent" id="divBaseInfo">
            <div class="row tableContent" style="border: none; padding-bottom: 30px;" id="showliuyan">
                <div id="control445" class="col-md-12" style="border: none;">
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
            <div class="nevershow">
                <div class="row">
                    <div class="col-md-4">
                        <div id="title1" class="leftbox">
                            <span id="Label11" data-type="SheetLabel" data-datafield="applicationNo" style="">贷款申请号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control1" class="rightbox">
                            <input id="applicationNo" type="text" data-datafield="applicationNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="divFullNameTitle" class="leftbox2">
                            <label id="lblFullNameTitle" data-type="SheetLabel" data-datafield="Originator.UserName" data-en_us="Originator" data-bindtype="OnlyVisiable" style="">发起人</label>
                        </div>
                        <div class="centerline2"></div>
                        <div id="divFullName" class="rightbox2">
                            <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style="">
                            </label>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="divOriginateOUNameTitle" class="leftbox3">
                            <label id="lblOriginateOUNameTitle" data-type="SheetLabel" data-datafield="Originator.OUName" data-en_us="Originate OUName" data-bindtype="OnlyVisiable" style="">所属组织</label>
                        </div>
                        <div class="centerline3"></div>
                        <div id="divOriginateOUName" class="rightbox3">
                            <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style="">
                            </select>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divBaseInfo1">
                <div class="row">
                    <div class="col-md-4">
                        <div id="divOriginateDateTitle" class="leftbox">
                            <label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                        </div>
                        <div class="centerline"></div>
                        <div id="divOriginateDate" class="rightbox">
                            <label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style="">
                            </label>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="divSequenceNoTitle" class="leftbox2">
                            <label id="lblSequenceNoTitle" data-type="SheetLabel" data-datafield="SequenceNo" data-en_us="SequenceNo" data-bindtype="OnlyVisiable" style="">流水号</label>
                        </div>
                        <div class="centerline2"></div>
                        <div id="divSequenceNo" class="rightbox2">
                            <label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style="">
                            </label>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title2" class="leftbox3">
                            <span id="Label12" data-type="SheetLabel" data-datafield="appTypeName" style="">申请类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control2" class="rightbox3">
                            <input id="Control12" type="text" data-datafield="appTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title3" class="leftbox">
                            <span id="Label13" data-type="SheetLabel" data-datafield="appCreateTime" style="">日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3" class="rightbox">
                            <input id="Control13" type="text" data-datafield="appCreateTime" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title4" class="leftbox2">
                            <span id="Label14" data-type="SheetLabel" data-datafield="companyName" style="">申请贷款公司</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control4" class="rightbox2">
                            <input id="Control14" type="text" data-datafield="companyName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title5" class="leftbox3">
                            <span id="Label15" data-type="SheetLabel" data-datafield="financialadvisorName" style="">金融顾问</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control5" class="rightbox3">
                            <input id="Control15" type="text" data-datafield="financialadvisorName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title6" class="leftbox">
                            <span id="Label16" data-type="SheetLabel" data-datafield="vehicleTypeName" style="">车辆类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control6" class="rightbox">
                            <input id="Control16" type="text" data-datafield="vehicleTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title7" class="leftbox2">
                            <span id="Label17" data-type="SheetLabel" data-datafield="provinceName" style="">经销商省</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control7" class="rightbox2">
                            <input id="Control17" type="text" data-datafield="provinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title8" class="leftbox3">
                            <span id="Label18" data-type="SheetLabel" data-datafield="dealercityName" style="">经销商城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control8" class="rightbox3">
                            <input id="Control18" type="text" data-datafield="dealercityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title9" class="leftbox">
                            <span id="Label19" data-type="SheetLabel" data-datafield="salesPersonName" style="">销售人员</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control9" class="rightbox">
                            <input id="Control19" type="text" data-datafield="salesPersonName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title10" class="leftbox2">
                            <span id="Label20" data-type="SheetLabel" data-datafield="showRoomName" style="">展厅</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control10" class="rightbox2">
                            <input id="Control20" type="text" data-datafield="showRoomName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title11" class="leftbox3">
                            <span id="Label21" data-type="SheetLabel" data-datafield="distributorName" style="">制造商</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control11" class="rightbox3">
                            <input id="Control21" type="text" data-datafield="distributorName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title12" class="leftbox">
                            <span id="Label22" data-type="SheetLabel" data-datafield="ContactNumber" style="">联系方式</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control12" class="rightbox">
                            <input id="Control22" type="text" data-datafield="ContactNumber" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title13" class="leftbox2">
                            <span id="Label23" data-type="SheetLabel" data-datafield="refCANumber" style="">申请参考号</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control13" class="rightbox2">
                            <input id="Control23" type="text" data-datafield="refCANumber" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title14" class="leftbox3">
                            <span id="Label24" data-type="SheetLabel" data-datafield="dealerName" style="">经销商</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control14" class="rightbox3">
                            <input id="Control24" type="text" data-datafield="dealerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title15" class="leftbox">
                            <span id="Label25" data-type="SheetLabel" data-datafield="userName" style="">账户名</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control15" class="rightbox">
                            <input id="Control25" type="text" data-datafield="userName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title19" class="leftbox2">
                            <span id="Label27" data-type="SheetLabel" data-datafield="applicant_type" style="">申请类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control19" class="rightbox2">
                            <input id="Control27" type="text" data-datafield="applicant_type" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" class="isshowdetail">
                <a href="javascript:void(0);" onclick="hideInfo('divBaseInfo1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label id="Label5" data-en_us="Sheet information">申请人信息</label>
        </div>
        <div class="divContent" id="divSQR">
            <div>
                <div class="row" style="display: none;">
                    <div id="title421" class="col-md-2">
                        <span id="Label380" data-type="SheetLabel" data-datafield="fkResult" style="">风控审核结果</span>
                    </div>
                    <div id="control421" class="leftbox">
                        <input id="Control380" type="text" data-datafield="fkResult" data-type="SheetTextBox" style="">
                        <a href="#" id="aclick" target="_blank" style="display: none;"></a>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title21" class="leftbox ">
                            <span id="Label29" data-type="SheetLabel" data-datafield="ThaiFirstName" style="">姓名（中文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control21" class="rightbox ">
                            <input id="Control29" type="text" data-datafield="ThaiFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title105" class="leftbox2">
                            <span id="Label105" data-type="SheetLabel" data-datafield="phoneNo" style="">电话</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control105" class="rightbox2">
                            <input id="Control105" type="text" data-datafield="phoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title20" class="leftbox3">
                            <span id="Label28" data-type="SheetLabel" data-datafield="IS_INACTIVE_IND" style="">是否隐藏共借人</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control20" class="rightbox3">
                            <input id="Control28" type="text" data-datafield="IS_INACTIVE_IND" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title23" class="leftbox">
                            <span id="Label31" data-type="SheetLabel" data-datafield="IdCardNo" style="">证件号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control23" class="rightbox">
                            <input id="ctrlIdCardNo" type="text" data-datafield="IdCardNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title27" class="leftbox2 ">
                            <span id="Label34" data-type="SheetLabel" data-datafield="HokouName" style="">户口所在地</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control27" class="rightbox2 ">
                            <textarea id="Control34" data-datafield="HokouName" data-type="SheetTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title167" class="leftbox3">
                            <span id="Label164" data-type="SheetLabel " data-datafield="Gjspouse" style="">共借人是否隐藏配偶</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control167" class="rightbox3">
                            <input id="Control164" type="text" data-datafield="Gjspouse" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title29" class="leftbox ">
                            <span id="Label35" data-type="SheetLabel" data-datafield="DateOfBirth" style="">出生日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control29" class="rightbox ">
                            <input id="Control35" type="text" data-datafield="DateOfBirth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title50" class="leftbox2 ">
                            <span id="Label56" data-type="SheetLabel" data-datafield="EducationName" style="">教育程度</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control50" class="rightbox2 ">
                            <input id="Control56" type="text" data-datafield="EducationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">
                            第三方黑名单记录
                        </div>
                        <div class="centerline3"></div>
                        <div class="rightbox3" id="sqrhmd">
                            <span>查询中</span>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title37" class="leftbox ">
                            <span id="Label43" data-type="SheetLabel" data-datafield="DrivingLicenseStatusName" style="">驾照状态</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control37" class="rightbox ">
                            <input id="Control43" type="text" data-datafield="DrivingLicenseStatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title24" class="leftbox2 ">
                            <span id="Label32" data-type="SheetLabel" data-datafield="MaritalStatusName" style="">婚姻状况</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control24" class="rightbox2 ">
                            <input id="Control32" type="text" data-datafield="MaritalStatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">
                            第三方外部信用记录
                        </div>
                        <div class="centerline3"></div>
                        <div class="rightbox3" id="sqrwbxy">
                            <span>查询中</span>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title83" class="leftbox1">
                            <span id="Label85" data-type="SheetLabel" data-datafield="currentLivingAddress" style="">申请人地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control83" class="rightbox1">
                            <textarea id="Control85" data-datafield="currentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title90" class="leftbox3 ">
                            <span id="Label90" data-type="SheetLabel" data-datafield="nativedistrict" style="">籍贯</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control90" class="rightbox3 ">
                            <input id="Control90" type="text" data-datafield="nativedistrict" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title85" class="leftbox1 ">
                            <span id="Label86" data-type="SheetLabel" data-datafield="AddressId" style="">户籍地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control85" class="rightbox1">
                            <textarea id="Control86" data-datafield="AddressId" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title92" class="leftbox3 ">
                            <span id="Label92" data-type="SheetLabel" data-datafield="hukouprovinceName" style="">户籍省份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control92" class="rightbox3 ">
                            <input id="Control92" type="text" data-datafield="hukouprovinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title65" class="leftbox1">
                            <span id="Label71" data-type="SheetLabel" data-datafield="ZjrCompanyName" style="">公司名称</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control65" class="rightbox1">
                            <input id="Control71" type="text" data-datafield="ZjrCompanyName" data-type="SheetTextBox" style="" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title63" class="leftbox3">
                            <span id="Label69" data-type="SheetLabel" data-datafield="lienee" style="">抵押人</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control63" class="rightbox3">
                            <input id="Control69" type="text" data-datafield="lienee" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title67" class="leftbox">
                            <span id="Label72" data-type="SheetLabel" data-datafield="BusinessTypeName" style="">企业性质</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control67" class="rightbox">
                            <input id="Control72" type="text" data-datafield="BusinessTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title71" class="leftbox2">
                            <span id="Label76" data-type="SheetLabel" data-datafield="phonenumber" style="">公司电话</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control71" class="rightbox2">
                            <input id="Control76" type="text" data-datafield="phonenumber" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    
                      <div class="col-md-4">
                        <div id="Div1" class="leftbox3 ">
                             <span id="Label448" data-type="SheetLabel" data-datafield="dycs" style="">抵押城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="Div3" class="rightbox3 " style="color:red;">
                           <input id="Control451" type="text" data-datafield="dydz" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div id="divSQR1">
                <div class="row">
                     <div class="col-md-4">
                        <div id="title64" class="leftbox ">
                            <span id="Label70" data-type="SheetLabel" data-datafield="ActualSalary" style="">月收入</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="Div4" class="rightbox ">
                            <input id="Text2" type="text" data-datafield="ActualSalary" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title70" class="leftbox2">
                            <span id="Label75" data-type="SheetLabel" data-datafield="companyCityName" style="">单位城市</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control70" class="rightbox2">
                            <input id="Control75" type="text" data-datafield="companyCityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title25" class="leftbox3">
                            <span id="Label33" data-type="SheetLabel" data-datafield="genderName" style="">性别</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control25" class="rightbox3">
                            <input id="Control33" type="text" data-datafield="genderName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title30" class="leftbox">
                            <span id="Label36" data-type="SheetLabel" data-datafield="CitizenshipName" style="">国籍</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control30001" class="rightbox">
                            <input id="Control36" type="text" data-datafield="CitizenshipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title31" class="leftbox2">
                            <span id="Label37" data-type="SheetLabel" data-datafield="IdCardIssueDate" style="">证件发行日</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control31" class="rightbox2">
                            <input id="Control37" type="text" data-datafield="IdCardIssueDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title32" class="leftbox3">
                            <span id="Label38" data-type="SheetLabel" data-datafield="IdCardExpiryDate" style="">证件到期日</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control32" class="rightbox3">
                            <input id="Control38" type="text" data-datafield="IdCardExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title33" class="leftbox">
                            <span id="Label39" data-type="SheetLabel" data-datafield="LicenseNo" style="">驾照号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control33" class="rightbox">
                            <input id="Control39" type="text" data-datafield="LicenseNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title34" class="leftbox2">
                            <span id="Label40" data-type="SheetLabel" data-datafield="LicenseExpiryDate" style="">驾照到期日</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control34" class="rightbox2">
                            <input id="Control40" type="text" data-datafield="LicenseExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title35" class="leftbox3">
                            <span id="Label41" data-type="SheetLabel" data-datafield="ThaiTitleName" style="">头衔</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control35" class="rightbox3">
                            <input id="Control41" type="text" data-datafield="ThaiTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title36" class="leftbox">
                            <span id="Label42" data-type="SheetLabel" data-datafield="TitleName" style="">头衔（英文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control36" class="rightbox">
                            <input id="Control42" type="text" data-datafield="TitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title38" class="leftbox2">
                            <span id="Label44" data-type="SheetLabel" data-datafield="EngFirstName" style="">名（英文）</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control38" class="rightbox2">
                            <input id="Control44" type="text" data-datafield="EngFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title39" class="leftbox3">
                            <span id="Label45" data-type="SheetLabel" data-datafield="EngMiddleName" style="">中间名字</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control39" class="rightbox3">
                            <input id="Control45" type="text" data-datafield="EngMiddleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title40" class="leftbox">
                            <span id="Label46" data-type="SheetLabel" data-datafield="EngLastName" style="">姓（英文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control40" class="rightbox">
                            <input id="Control46" type="text" data-datafield="EngLastName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title41" class="leftbox2">
                            <span id="Label47" data-type="SheetLabel" data-datafield="FormerName" style="">曾用名</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control41" class="rightbox2">
                            <input id="Control47" type="text" data-datafield="FormerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title42" class="leftbox3">
                            <span id="Label48" data-type="SheetLabel" data-datafield="NationName" style="">民族</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control42" class="rightbox3">
                            <input id="Control48" type="text" data-datafield="NationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title43" class="leftbox">
                            <span id="Label49" data-type="SheetLabel" data-datafield="IssuingAuthority" style="">签发机关</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control43" class="rightbox">
                            <input id="Control49" type="text" data-datafield="IssuingAuthority" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title44" class="leftbox2">
                            <span id="Label50" data-type="SheetLabel" data-datafield="NumberOfDependents" style="">供养人数</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control44" class="rightbox2">
                            <input id="Control50" type="text" data-datafield="NumberOfDependents" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title45" class="leftbox3">
                            <span id="Label51" data-type="SheetLabel" data-datafield="HouseOwnerName" style="">房产所有人</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control45" class="rightbox3">
                            <input id="Control51" type="text" data-datafield="HouseOwnerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title46" class="leftbox">
                            <span id="Label52" data-type="SheetLabel" data-datafield="NoOfFamilyMembers" style="">家庭人数</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control46" class="rightbox">
                            <input id="Control52" type="text" data-datafield="NoOfFamilyMembers" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title47" class="leftbox2">
                            <span id="Label53" data-type="SheetLabel" data-datafield="ChildrenFlag" style="">是否是子女</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control47" class="rightbox2">
                            <input id="Control53" type="text" data-datafield="ChildrenFlag" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title48" class="leftbox3">
                            <span id="Label54" data-type="SheetLabel" data-datafield="EmploymentTypeName" style="">雇员类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control48" class="rightbox3">
                            <input id="Control54" type="text" data-datafield="EmploymentTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title49" class="leftbox">
                            <span id="Label55" data-type="SheetLabel" data-datafield="EmailAddress" style="">邮箱地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control49" class="rightbox">
                            <input id="Control55" type="text" data-datafield="EmailAddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title51" class="leftbox2">
                            <span id="Label57" data-type="SheetLabel" data-datafield="IndustryTypeName" style="">行业类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control51" class="rightbox2">
                            <input id="Control57" type="text" data-datafield="IndustryTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title52" class="leftbox3">
                            <span id="Label58" data-type="SheetLabel" data-datafield="IndustrySubTypeName" style="">行业子类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control52" class="rightbox3">
                            <input id="Control58" type="text" data-datafield="IndustrySubTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title53" class="leftbox">
                            <span id="Label59" data-type="SheetLabel" data-datafield="OccupationName" style="">职业类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control53" class="rightbox">
                            <input id="Control59" type="text" data-datafield="OccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title54" class="leftbox2">
                            <span id="Label60" data-type="SheetLabel" data-datafield="SubOccupationName" style="">职业子类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control54" class="rightbox2">
                            <input id="Control60" type="text" data-datafield="SubOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title55" class="leftbox3">
                            <span id="Label61" data-type="SheetLabel" data-datafield="DesginationName" style="">职位</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control55" class="rightbox3">
                            <input id="Control61" type="text" data-datafield="DesginationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title56" class="leftbox">
                            <span id="Label62" data-type="SheetLabel" data-datafield="JobGroupName" style="">工作组</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control56" class="rightbox">
                            <input id="Control62" type="text" data-datafield="JobGroupName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title57" class="leftbox2">
                            <span id="Label63" data-type="SheetLabel" data-datafield="SalaryRangeName" style="">估计收入</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control57" class="rightbox2">
                            <input id="Control63" type="text" data-datafield="SalaryRangeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title58" class="leftbox3">
                            <span id="Label64" data-type="SheetLabel" data-datafield="Consent" style="">同意</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control58" class="rightbox3">
                            <input id="Control64" type="text" data-datafield="Consent" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title59" class="leftbox">
                            <span id="Label65" data-type="SheetLabel" data-datafield="VIPInd" style="">贵宾</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control59" class="rightbox">
                            <input id="Control65" type="text" data-datafield="VIPInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title60" class="leftbox2">
                            <span id="Label66" data-type="SheetLabel" data-datafield="StaffInd" style="">工作人员</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control60" class="rightbox2">
                            <input id="Control66" type="text" data-datafield="StaffInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title61" class="leftbox3">
                            <span id="Label67" data-type="SheetLabel" data-datafield="BlackListNoRecordInd" style="">黑名单无记录</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control61" class="rightbox3">
                            <input id="Control67" type="text" data-datafield="BlackListNoRecordInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title62" class="leftbox">
                            <span id="Label68" data-type="SheetLabel" data-datafield="BlacklistInd" style="">外部信用记录</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control62" class="rightbox ">
                            <input id="Control68" type="text" data-datafield="BlacklistInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title68" class="leftbox2">
                            <span id="Label73" data-type="SheetLabel" data-datafield="positionName" style="">职位</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control68" class="rightbox2">
                            <input id="Control73" type="text" data-datafield="positionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title69" class="leftbox3">
                            <span id="Label74" data-type="SheetLabel" data-datafield="comapnyProvinceName" style="">单位省份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control69" class="rightbox3">
                            <input id="Control74" type="text" data-datafield="comapnyProvinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title72" class="leftbox">
                            <span id="Label77" data-type="SheetLabel" data-datafield="Fax" style="">传真</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control72" class="rightbox">
                            <input id="Control77" type="text" data-datafield="Fax" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-8">
                        <div id="title73" class="leftbox1">
                            <span id="Label78" data-type="SheetLabel" data-datafield="companyAddress" style="">公司地址 </span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control73" class="rightbox1">
                            <textarea id="Control78" data-datafield="companyAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title75" class="leftbox">
                            <span id="Label79" data-type="SheetLabel" data-datafield="EmployerType" style="">雇主类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control75" class="rightbox">
                            <input id="Control79" type="text" data-datafield="EmployerType" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title76" class="leftbox2">
                            <span id="Label80" data-type="SheetLabel" data-datafield="NoOfEmployees" style="">雇员人数</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control76" class="rightbox2">
                            <input id="Control80" type="text" data-datafield="NoOfEmployees" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title77" class="leftbox3">
                            <span id="Label81" data-type="SheetLabel" data-datafield="JobDescription" style="">工作描述 </span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control77" class="rightbox3">
                            <input id="Control81" type="text" data-datafield="JobDescription" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title79" class="leftbox">
                            <span id="Label83" data-type="SheetLabel" data-datafield="timeinmonth" style="">工作年限（月） </span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control79" class="rightbox">
                            <input id="Control83" type="text" data-datafield="timeinmonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-8">
                        <div id="title81" class="leftbox1">
                            <span id="Label84" data-type="SheetLabel" data-datafield="companyComments" style="">公司评论 </span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control81" class="rightbox1">
                            <textarea id="Control84" data-datafield="companyComments" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title87" class="leftbox">
                            <span id="Label87" data-type="SheetLabel" data-datafield="defaultmailingaddress" style="">默认邮寄地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control87" class="rightbox">
                            <input id="Control87" type="text" data-datafield="defaultmailingaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title88" class="leftbox2">
                            <span id="Label88" data-type="SheetLabel" data-datafield="hukouaddress" style="">户籍地址</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control88" class="rightbox2">
                            <input id="Control88" type="text" data-datafield="hukouaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title89" class="leftbox3">
                            <span id="Label89" data-type="SheetLabel" data-datafield="countryName" style="">国家</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control89" class="rightbox3">
                            <input id="Control89" type="text" data-datafield="countryName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title91" class="leftbox">
                            <span id="Label91" data-type="SheetLabel" data-datafield="birthpalaceprovince" style="">出生地省市县（区）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control91" class="rightbox">
                            <input id="Control91" type="text" data-datafield="birthpalaceprovince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title93" class="leftbox2">
                            <span id="Label93" data-type="SheetLabel" data-datafield="hukoucityName" style="">户籍城市</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control93" class="rightbox2">
                            <input id="Control93" type="text" data-datafield="hukoucityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title94" class="leftbox3">
                            <span id="Label94" data-type="SheetLabel" data-datafield="companypostCode" style="">公司邮编</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control94" class="rightbox3">
                            <input id="Control94" type="text" data-datafield="companypostCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title95" class="leftbox">
                            <span id="Label95" data-type="SheetLabel" data-datafield="postcode" style="">邮编</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control95" class="rightbox">
                            <input id="Control95" type="text" data-datafield="postcode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title96" class="leftbox2">
                            <span id="Label96" data-type="SheetLabel" data-datafield="addresstypeName" style="">地址类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control96" class="rightbox2">
                            <input id="Control96" type="text" data-datafield="addresstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title97" class="leftbox3">
                            <span id="Label97" data-type="SheetLabel" data-datafield="addressstatusName" style="">地址状态</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control97" class="rightbox3">
                            <input id="Control97" type="text" data-datafield="addressstatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title98" class="leftbox">
                            <span id="Label98" data-type="SheetLabel" data-datafield="propertytypeName" style="">房产类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control98" class="rightbox">
                            <input id="Control98" type="text" data-datafield="propertytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title99" class="leftbox2">
                            <span id="Label99" data-type="SheetLabel" data-datafield="residencetypeName" style="">住宅类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control99" class="rightbox2">
                            <input id="Control99" type="text" data-datafield="residencetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title100" class="leftbox3">
                            <span id="Label100" data-type="SheetLabel" data-datafield="livingsince" style="">开始居住日期</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control100" class="rightbox3">
                            <input id="Control100" type="text" data-datafield="livingsince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title101" class="leftbox">
                            <span id="Label101" data-type="SheetLabel" data-datafield="homevalue" style="">房屋价值（万元)</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control101" class="rightbox">
                            <input id="Control101" type="text" data-datafield="homevalue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title102" class="leftbox2">
                            <span id="Label102" data-type="SheetLabel" data-datafield="stayinyear" style="">居住年限</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control102" class="rightbox2">
                            <input id="Control102" type="text" data-datafield="stayinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title103" class="leftbox3">
                            <span id="Label103" data-type="SheetLabel" data-datafield="countrycodeName" style="">国家代码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control103" class="rightbox3">
                            <input id="Control103" type="text" data-datafield="countrycodeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title104" class="leftbox">
                            <span id="Label104" data-type="SheetLabel" data-datafield="areaCode" style="">地区代码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control104" class="rightbox">
                            <input id="Control104" type="text" data-datafield="areaCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title106" class="leftbox2">
                            <span id="Label106" data-type="SheetLabel" data-datafield="extension" style="">分机</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control106" class="rightbox2">
                            <input id="Control106" type="text" data-datafield="extension" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title107" class="leftbox3">
                            <span id="Label107" data-type="SheetLabel" data-datafield="phonetypeName" style="">电话类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control107" class="rightbox3">
                            <input id="Control107" type="text" data-datafield="phonetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title113" class="leftbox">
                            <span id="Label113" data-type="SheetLabel" data-datafield="name1" style="">名称</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control113" class="rightbox">
                            <input id="Control113" type="text" data-datafield="name1" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title114" class="leftbox2">
                            <span id="Label114" data-type="SheetLabel" data-datafield="relationshipName" style="">关系</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control114" class="rightbox2">
                            <input id="Control114" type="text" data-datafield="relationshipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title115" class="leftbox3">
                            <span id="Label115" data-type="SheetLabel" data-datafield="address" style="">联系人地址</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control115" class="rightbox3">
                            <input id="Control115" type="text" data-datafield="address" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title116" class="leftbox">
                            <span id="Label116" data-type="SheetLabel" data-datafield="lxprovinceCode" style="">联系人省份</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control116" class="rightbox">
                            <input id="Control116" type="text" data-datafield="lxprovinceCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title117" class="leftbox2">
                            <span id="Label117" data-type="SheetLabel" data-datafield="lxcityName" style="">联系人城市</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control117" class="rightbox2">
                            <input id="Control117" type="text" data-datafield="lxcityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title118" class="leftbox3">
                            <span id="Label118" data-type="SheetLabel" data-datafield="lxpostCode" style="">联系人邮编</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control118" class="rightbox3">
                            <input id="Control118" type="text" data-datafield="lxpostCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title121" class="leftbox">
                            <span id="Label120" data-type="SheetLabel" data-datafield="lxphoneNo" style="">联系人电话</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control121" class="rightbox">
                            <input id="Control120" type="text" data-datafield="lxphoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title122" class="leftbox2">
                            <span id="Label121" data-type="SheetLabel" data-datafield="lxmobile" style="">联系人手机</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control122" class="rightbox2">
                            <input id="Control121" type="text" data-datafield="lxmobile" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title119" class="leftbox3">
                            <span id="Label119" data-type="SheetLabel" data-datafield="lxhokouName" style="">联系人户口所在地</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control119" class="rightbox3">
                            <textarea id="Control119" data-datafield="lxhokouName" data-type="SheetTextBox" style="">					</textarea>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title78" class="leftbox">
                            <span id="Label82" data-type="SheetLabel" data-datafield="timeinyear" style="">工作年限（年）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control78" class="rightbox">
                            <input id="Control82" type="text" data-datafield="timeinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                     <div class="col-md-4">
                        <div id="title22" class="leftbox2">
                            <span id="Label30" data-type="SheetLabel" data-datafield="IdCarTypeName" style="">证件类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="Div5" class="rightbox2">
                            <input id="Control30" type="text" data-datafield="IdCarTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                <a href="javascript:void(0);" onclick="hideInfo('divSQR1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divGJR',this)">
            <label id="Label6" data-en_us="Sheet information">共借人信息</label>
        </div>
        <div class="divContent" id="divGJR">
            <div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title123" class="leftbox">
                            <span id="Label122" data-type="SheetLabel" data-datafield="GjThaiFirstName" style="">姓名（中文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control123" class="rightbox">
                            <input id="Control122" type="text" data-datafield="GjThaiFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title209" class="leftbox2">
                            <span id="Label201" data-type="SheetLabel" data-datafield="GjphoneNo" style="">电话</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control209" class="rightbox2">
                            <input id="Control201" type="text" data-datafield="GjphoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title125" class="leftbox3">
                            <span id="Label124" data-type="SheetLabel" data-datafield="GjIdCardNo" style="">证件号码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control125" class="rightbox3">
                            <input id="Control124" type="text" data-datafield="GjIdCardNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title165" class="leftbox">
                            <span id="Label162" data-type="SheetLabel" data-datafield="Gjlienee" style="">抵押人</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control165" class="rightbox">
                            <input id="Control162" type="text" data-datafield="Gjlienee" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title129" class="leftbox2">
                            <span id="Label127" data-type="SheetLabel" data-datafield="GjHokouName" style="">户口所在地</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control129" class="rightbox2">
                            <textarea id="Control127" data-datafield="GjHokouName" data-type="SheetTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title168" class="leftbox3">
                            <span id="Label165" data-type="SheetLabel" data-datafield="GjrelationShipName" style="">共借人和主借人的关系</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control168" class="rightbox3">
                            <input id="Control165" type="text" data-datafield="GjrelationShipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title131" class="leftbox">
                            <span id="Label128" data-type="SheetLabel" data-datafield="GjDateOfBirth" style="">出生日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control131" class="rightbox">
                            <input id="Control128" type="text" data-datafield="GjDateOfBirth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title152" class="leftbox2">
                            <span id="Label149" data-type="SheetLabel" data-datafield="GjEducationName" style="">教育程度</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control152" class="rightbox2">
                            <input id="Control149" type="text" data-datafield="GjEducationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">
                            第三方黑名单无记录
                        </div>
                        <div class="centerline3"></div>
                        <div class="rightbox3" id="gjrhmd">
                            <span>查询中</span>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title139" class="leftbox">
                            <span id="Label136" data-type="SheetLabel" data-datafield="GjDrivingLicenseStatusName" style="">驾照状态</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control139" class="rightbox">
                            <input id="Control136" type="text" data-datafield="GjDrivingLicenseStatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title126" class="leftbox2">
                            <span id="Label125" data-type="SheetLabel" data-datafield="GjMaritalStatusName" style="">婚姻状况</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control126" class="rightbox2">
                            <input id="Control125" type="text" data-datafield="GjMaritalStatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">
                            第三方外部信用记录
                        </div>
                        <div class="centerline3"></div>
                        <div class="rightbox3" id="gjrwbxy">
                            <span>查询中</span>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title187" class="leftbox1">
                            <span id="Label181" data-type="SheetLabel" data-datafield="GjcurrentLivingAddress" style="">地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control187" class="rightbox1">
                            <textarea id="Control181" data-datafield="GjcurrentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title194" class="leftbox3">
                            <span id="Label186" data-type="SheetLabel" data-datafield="Gjnativedistrict" style="">籍贯</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control194" class="rightbox3">
                            <input id="Control186" type="text" data-datafield="Gjnativedistrict" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title189" class="leftbox1">
                            <span id="Label182" data-type="SheetLabel" data-datafield="GjAddressId" style="">户籍地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control189" class="rightbox1">
                            <textarea id="Control182" data-datafield="GjAddressId" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title196" class="leftbox3">
                            <span id="Label188" data-type="SheetLabel" data-datafield="GjhukouprovinceName" style="">共借人省份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control196" class="rightbox3">
                            <input id="Control188" type="text" data-datafield="GjhukouprovinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title169" class="leftbox1">
                            <span id="Label166" data-type="SheetLabel" data-datafield="GjcompanyName" style="">公司名称</span>
                        </div>
                        <div class="centerline1" style="left: 17.5%;"></div>
                        <div id="control169" class="rightbox1">
                            <input id="Control166" type="text" data-datafield="GjcompanyName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title170" class="leftbox3">
                            <span id="Label167" data-type="SheetLabel" data-datafield="GjBusinessTypeName" style="">企业性质</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control170" class="rightbox3">
                            <input id="Control167" type="text" data-datafield="GjBusinessTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title174" class="leftbox">
                            <span id="Label171" data-type="SheetLabel" data-datafield="Gjphonenumber" style="">公司电话 </span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control174" class="rightbox">
                            <input id="Control171" type="text" data-datafield="Gjphonenumber" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title183" class="leftbox2">
                            <span id="Label178" data-type="SheetLabel" data-datafield="Gjtimeinyear" style="">工作年限（年）</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control183" class="rightbox2">
                            <input id="Control178" type="text" data-datafield="Gjtimeinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title166" class="leftbox3">
                            <span id="Label163" data-type="SheetLabel" data-datafield="GjActualSalary" style="">共借人月收入</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control166" class="rightbox3">
                            <input id="Control163" type="text" data-datafield="GjActualSalary" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div id="divGJR1">
                <div class="row">
                    <div class="col-md-4">
                        <div id="title124" class="leftbox">
                            <span id="Label123" data-type="SheetLabel" data-datafield="GjIdCarTypeName" style="">共借人证件类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control124" class="rightbox">
                            <input id="Control123" type="text" data-datafield="GjIdCarTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title127" class="leftbox2">
                            <span id="Label126" data-type="SheetLabel" data-datafield="GjgenderName" style="">共借人性别</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control127" class="rightbox2">
                            <input id="Control126" type="text" data-datafield="GjgenderName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title132" class="leftbox3">
                            <span id="Label129" data-type="SheetLabel" data-datafield="GjCitizenshipName" style="">共借人国籍</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control132" class="rightbox3">
                            <input id="Control129" type="text" data-datafield="GjCitizenshipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title133" class="leftbox">
                            <span id="Label130" data-type="SheetLabel" data-datafield="GjIdCardIssueDate" style="">共借人证件发行日</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control133" class="rightbox">
                            <input id="Control130" type="text" data-datafield="GjIdCardIssueDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title134" class="leftbox2">
                            <span id="Label131" data-type="SheetLabel" data-datafield="GjIdCardExpiryDate" style="">共借人证件到期日</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control134" class="rightbox2">
                            <input id="Control131" type="text" data-datafield="GjIdCardExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title135" class="leftbox3">
                            <span id="Label132" data-type="SheetLabel" data-datafield="GjLicenseNo" style="">共借人驾照号码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control135" class="rightbox3">
                            <input id="Control132" type="text" data-datafield="GjLicenseNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title136" class="leftbox">
                            <span id="Label133" data-type="SheetLabel" data-datafield="GjLicenseExpiryDate" style="">共借人驾照到期日</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control136" class="rightbox">
                            <input id="Control133" type="text" data-datafield="GjLicenseExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title137" class="leftbox2">
                            <span id="Label134" data-type="SheetLabel" data-datafield="GjThaiTitleName" style="">共借人头衔</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control137" class="rightbox2">
                            <input id="Control134" type="text" data-datafield="GjThaiTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title138" class="leftbox3">
                            <span id="Label135" data-type="SheetLabel" data-datafield="GjTitleName" style="">共借人头衔（英文）</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control138" class="rightbox3">
                            <input id="Control135" type="text" data-datafield="GjTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title140" class="leftbox">
                            <span id="Label137" data-type="SheetLabel" data-datafield="GjEngFirstName" style="">共借人名（英文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control140" class="rightbox">
                            <input id="Control137" type="text" data-datafield="GjEngFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title141" class="leftbox2">
                            <span id="Label138" data-type="SheetLabel" data-datafield="GjEngLastName" style="">共借人姓（英文）</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control141" class="rightbox2">
                            <input id="Control138" type="text" data-datafield="GjEngLastName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title142" class="leftbox3">
                            <span id="Label139" data-type="SheetLabel" data-datafield="GjEngMiddleName" style="">共借人中间名字</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control142" class="rightbox3">
                            <input id="Control139" type="text" data-datafield="GjEngMiddleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title143" class="leftbox">
                            <span id="Label140" data-type="SheetLabel" data-datafield="GjFormerName" style="">共借人曾用名</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control143" class="rightbox">
                            <input id="Control140" type="text" data-datafield="GjFormerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title144" class="leftbox2">
                            <span id="Label141" data-type="SheetLabel" data-datafield="GjNationName" style="">共借人民族</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control144" class="rightbox2">
                            <input id="Control141" type="text" data-datafield="GjNationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title145" class="leftbox3">
                            <span id="Label142" data-type="SheetLabel" data-datafield="GjIssuingAuthority" style="">共借人签发机关</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control145" class="rightbox3">
                            <input id="Control142" type="text" data-datafield="GjIssuingAuthority" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title146" class="leftbox">
                            <span id="Label143" data-type="SheetLabel" data-datafield="GjNumberOfDependents" style="">共借人供养人数</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control146" class="rightbox">
                            <input id="Control143" type="text" data-datafield="GjNumberOfDependents" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title147" class="leftbox2">
                            <span id="Label144" data-type="SheetLabel" data-datafield="GjHouseOwnerName" style="">共借人房产所有人</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control147" class="rightbox2">
                            <input id="Control144" type="text" data-datafield="GjHouseOwnerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title148" class="leftbox3">
                            <span id="Label145" data-type="SheetLabel" data-datafield="GjNoOfFamilyMembers" style="">共借人家庭人数</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control148" class="rightbox3">
                            <input id="Control145" type="text" data-datafield="GjNoOfFamilyMembers" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title149" class="leftbox">
                            <span id="Label146" data-type="SheetLabel" data-datafield="GjChildrenFlag" style="">共借人是否是子女</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control149" class="rightbox">
                            <input id="Control146" type="text" data-datafield="GjChildrenFlag" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title150" class="leftbox2">
                            <span id="Label147" data-type="SheetLabel" data-datafield="GjEmploymentTypeName" style="">共借人雇员类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control150" class="rightbox2">
                            <input id="Control147" type="text" data-datafield="GjEmploymentTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title151" class="leftbox3">
                            <span id="Label148" data-type="SheetLabel" data-datafield="GjEmailAddress" style="">共借人邮箱地址</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control151" class="rightbox3">
                            <input id="Control148" type="text" data-datafield="GjEmailAddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title153" class="leftbox">
                            <span id="Label150" data-type="SheetLabel" data-datafield="GjIndustryTypeName" style="">共借人行业类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control153" class="rightbox">
                            <input id="Control150" type="text" data-datafield="GjIndustryTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title154" class="leftbox2">
                            <span id="Label151" data-type="SheetLabel" data-datafield="GjIndustrySubTypeName" style="">共借人行业子类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control154" class="rightbox2">
                            <input id="Control151" type="text" data-datafield="GjIndustrySubTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title155" class="leftbox3">
                            <span id="Label152" data-type="SheetLabel" data-datafield="GjOccupationName" style="">共借人职业类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control155" class="rightbox3">
                            <input id="Control152" type="text" data-datafield="GjOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title156" class="leftbox">
                            <span id="Label153" data-type="SheetLabel" data-datafield="GjSubOccupationName" style="">共借人职业子类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control156" class="rightbox">
                            <input id="Control153" type="text" data-datafield="GjSubOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title157" class="leftbox2">
                            <span id="Label154" data-type="SheetLabel" data-datafield="GjDesginationName" style="">共借人职位</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control157" class="rightbox2">
                            <input id="Control154" type="text" data-datafield="GjDesginationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title158" class="leftbox3">
                            <span id="Label155" data-type="SheetLabel" data-datafield="GjJobGroupName" style="">共借人工作组</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control158" class="rightbox3">
                            <input id="Control155" type="text" data-datafield="GjJobGroupName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title159" class="leftbox">
                            <span id="Label156" data-type="SheetLabel" data-datafield="GjSalaryRangeName" style="">共借人估计收入</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control159" class="rightbox">
                            <input id="Control156" type="text" data-datafield="GjSalaryRangeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title160" class="leftbox2">
                            <span id="Label157" data-type="SheetLabel" data-datafield="GjConsent" style="">共借人同意</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control160" class="rightbox2">
                            <input id="Control157" type="text" data-datafield="GjConsent" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title161" class="leftbox3">
                            <span id="Label158" data-type="SheetLabel" data-datafield="GjVIPInd" style="">共借人贵宾</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control161" class="rightbox3">
                            <input id="Control158" type="text" data-datafield="GjVIPInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title162" class="leftbox">
                            <span id="Label159" data-type="SheetLabel" data-datafield="GjStaffInd" style="">共借人工作人员</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control162" class="rightbox">
                            <input id="Control159" type="text" data-datafield="GjStaffInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title163" class="leftbox2">
                            <span id="Label160" data-type="SheetLabel" data-datafield="GjBlackListNoRecordInd" style="">共借人黑名单无记录</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control163" class="rightbox2">
                            <input id="Control160" type="text" data-datafield="GjBlackListNoRecordInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title164" class="leftbox3">
                            <span id="Label161" data-type="SheetLabel" data-datafield="GjBlacklistInd" style="">共借人外部信用记录</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control164" class="rightbox3">
                            <input id="Control161" type="text" data-datafield="GjBlacklistInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title171" class="leftbox">
                            <span id="Label168" data-type="SheetLabel" data-datafield="GjpositionName" style="">共借人职位</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control171" class="rightbox">
                            <input id="Control168" type="text" data-datafield="GjpositionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title172" class="leftbox2">
                            <span id="Label169" data-type="SheetLabel" data-datafield="GjProvinceName" style="">共借人公司省份</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control172" class="rightbox2">
                            <input id="Control169" type="text" data-datafield="GjProvinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title175" class="leftbox3">
                            <span id="Label172" data-type="SheetLabel" data-datafield="GjFax" style="">共借人公司传真</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control175" class="rightbox3">
                            <input id="Control172" type="text" data-datafield="GjFax" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div class="col-md-8">
                        <div id="title177" class="leftbox1">
                            <span id="Label173" data-type="SheetLabel" data-datafield="GjcompanyAddress" style="">共借人公司地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control177" class="rightbox1">
                            <textarea id="Control173" data-datafield="GjcompanyAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title179" class="leftbox3">
                            <span id="Label174" data-type="SheetLabel" data-datafield="GjcompanypostCode" style="">共借人公司邮编</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control179" class="rightbox3">
                            <input id="Control174" type="text" data-datafield="GjcompanypostCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title180" class="leftbox">
                            <span id="Label175" data-type="SheetLabel" data-datafield="GjEmployerType" style="">共借人雇主类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control180" class="rightbox">
                            <input id="Control175" type="text" data-datafield="GjEmployerType" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title181" class="leftbox2">
                            <span id="Label176" data-type="SheetLabel" data-datafield="GjNoOfEmployees" style="">共借人雇员人数</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control181" class="rightbox2">
                            <input id="Control176" type="text" data-datafield="GjNoOfEmployees" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title182" class="leftbox3">
                            <span id="Label177" data-type="SheetLabel" data-datafield="GjJobDescription" style="">共借人工作描述</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control182" class="rightbox3">
                            <input id="Control177" type="text" data-datafield="GjJobDescription" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title184" class="leftbox">
                            <span id="Label179" data-type="SheetLabel" data-datafield="Gjtimeinmonth" style="">共借人工作年限（月）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control184" class="rightbox">
                            <input id="Control179" type="text" data-datafield="Gjtimeinmonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-8">
                        <div id="title185" class="leftbox1">
                            <span id="Label180" data-type="SheetLabel" data-datafield="GjcompanyComments" style="">共借人公司评论</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control185" class="rightbox1">
                            <textarea id="Control180" data-datafield="GjcompanyComments" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title191" class="leftbox">
                            <span id="Label183" data-type="SheetLabel" data-datafield="Gjdefaultmailingaddress" style="">共借人默认邮寄地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control191" class="rightbox">
                            <input id="Control183" type="text" data-datafield="Gjdefaultmailingaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title192" class="leftbox2">
                            <span id="Label184" data-type="SheetLabel" data-datafield="Gjhukouaddress" style="">共借人户籍地址</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control192" class="rightbox2">
                            <input id="Control184" type="text" data-datafield="Gjhukouaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title193" class="leftbox3">
                            <span id="Label185" data-type="SheetLabel" data-datafield="GjcountryName" style="">共借人国家</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control193" class="rightbox3">
                            <input id="Control185" type="text" data-datafield="GjcountryName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title195" class="leftbox">
                            <span id="Label187" data-type="SheetLabel" data-datafield="Gjbirthpalaceprovince" style="">共借人出生地省市县（区）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control195" class="rightbox">
                            <input id="Control187" type="text" data-datafield="Gjbirthpalaceprovince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title197" class="leftbox2">
                            <span id="Label189" data-type="SheetLabel" data-datafield="GjhukoucityName" style="">共借人城市</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control197" class="rightbox2">
                            <input id="Control189" type="text" data-datafield="GjhukoucityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title198" class="leftbox3">
                            <span id="Label190" data-type="SheetLabel" data-datafield="Gjpostcode" style="">共借人邮编</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control198" class="rightbox3">
                            <input id="Control190" type="text" data-datafield="Gjpostcode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title199" class="leftbox">
                            <span id="Label191" data-type="SheetLabel" data-datafield="GjaddresstypeName" style="">共借人地址类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control199" class="rightbox">
                            <input id="Control191" type="text" data-datafield="GjaddresstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title200" class="leftbox2">
                            <span id="Label192" data-type="SheetLabel" data-datafield="GjaddressstatusName" style="">共借人地址状态</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control200" class="rightbox2">
                            <input id="Control192" type="text" data-datafield="GjaddressstatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title201" class="leftbox3">
                            <span id="Label193" data-type="SheetLabel" data-datafield="GjpropertytypeName" style="">共借人房产类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control201" class="rightbox3">
                            <input id="Control193" type="text" data-datafield="GjpropertytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title202" class="leftbox">
                            <span id="Label194" data-type="SheetLabel" data-datafield="GjresidencetypeName" style="">共借人住宅类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control202" class="rightbox">
                            <input id="Control194" type="text" data-datafield="GjresidencetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title203" class="leftbox2">
                            <span id="Label195" data-type="SheetLabel" data-datafield="Gjlivingsince" style="">共借人开始居住日期</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control203" class="rightbox2">
                            <input id="Control195" type="text" data-datafield="Gjlivingsince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title204" class="leftbox3">
                            <span id="Label196" data-type="SheetLabel" data-datafield="Gjhomevalue" style="">共借人房屋价值（万元)</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control204" class="rightbox3">
                            <input id="Control196" type="text" data-datafield="Gjhomevalue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title205" class="leftbox">
                            <span id="Label197" data-type="SheetLabel" data-datafield="Gjstayinyear" style="">共借人居住年限</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control205" class="rightbox">
                            <input id="Control197" type="text" data-datafield="Gjstayinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title206" class="leftbox2">
                            <span id="Label198" data-type="SheetLabel" data-datafield="Gjstayinmonth" style="">共借人居住月份</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control206" class="rightbox2">
                            <input id="Control198" type="text" data-datafield="Gjstayinmonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title207" class="leftbox3">
                            <span id="Label199" data-type="SheetLabel" data-datafield="GjcountrycodeName" style="">共借人国家代码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control207" class="rightbox3">
                            <input id="Control199" type="text" data-datafield="GjcountrycodeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title208" class="leftbox">
                            <span id="Label200" data-type="SheetLabel" data-datafield="GjareaCode" style="">共借人地区代码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control208" class="rightbox">
                            <input id="Control200" type="text" data-datafield="GjareaCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title210" class="leftbox2">
                            <span id="Label202" data-type="SheetLabel" data-datafield="Gjextension" style="">共借人分机</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control210" class="rightbox2">
                            <input id="Control202" type="text" data-datafield="Gjextension" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title211" class="leftbox3">
                            <span id="Label203" data-type="SheetLabel" data-datafield="GjphonetypeName" style="">共借人电话类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control211" class="rightbox3">
                            <input id="Control203" type="text" data-datafield="GjphonetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title173" class="leftbox">
                            <span id="Label170" data-type="SheetLabel" data-datafield="GjCityName" style="">单位城市</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control173" class="rightbox">
                            <input id="Control170" type="text" data-datafield="GjCityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                <a href="javascript:void(0);" onclick="hideInfo('divGJR1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divDBR',this)">
            <label id="Label7" data-en_us="Sheet information">担保人信息</label>
        </div>
        <div class="divContent" id="divDBR">
            <div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title217" class="leftbox">
                            <span id="Label209" data-type="SheetLabel" data-datafield="DbThaiFirstName" style="">姓名（中文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control217" class="rightbox">
                            <input id="Control209" type="text" data-datafield="DbThaiFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title301" class="leftbox2">
                            <span id="Label285" data-type="SheetLabel" data-datafield="DbphoneNo" style="">电话</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control301" class="rightbox2">
                            <input id="Control285" type="text" data-datafield="DbphoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title219" class="leftbox3">
                            <span id="Label211" data-type="SheetLabel" data-datafield="DbIdCardNo" style="">证件号码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control219" class="rightbox3">
                            <input id="Control211" type="text" data-datafield="DbIdCardNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title258" class="leftbox">
                            <span id="Label248" data-type="SheetLabel" data-datafield="Dblienee" style="">抵押人</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control258" class="rightbox">
                            <input id="Control248" type="text" data-datafield="Dblienee" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title223" class="leftbox2">
                            <span id="Label214" data-type="SheetLabel" data-datafield="DbHokouName" style="">户口所在地</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control223" class="rightbox2">
                            <textarea id="Control214" data-datafield="DbHokouName" data-type="SheetTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title331" class="leftbox3">
                            <span id="Label315" data-type="SheetLabel" data-datafield="guranteeOption" style="">第三方担保</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control331" class="rightbox3">
                            <input id="Control315" type="text" data-datafield="guranteeOption" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title225" class="leftbox">
                            <span id="Label215" data-type="SheetLabel" data-datafield="DbDateOfBirth" style="">出生日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control225" class="rightbox">
                            <input id="Control215" type="text" data-datafield="DbDateOfBirth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title245" class="leftbox2">
                            <span id="Label235" data-type="SheetLabel" data-datafield="DbEducationName" style="">教育程度 </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control245" class="rightbox2">
                            <input id="Control235" type="text" data-datafield="DbEducationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">
                            第三方黑名单无记录
                        </div>
                        <div class="centerline3"></div>
                        <div class="rightbox3" id="dbrhmd">
                            <span>查询中</span>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title233" class="leftbox">
                            <span id="Label223" data-type="SheetLabel" data-datafield="DbDrivingLicenseStatusName" style="">驾照状态</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control233" class="rightbox">
                            <input id="Control223" type="text" data-datafield="DbDrivingLicenseStatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title220" class="leftbox2">
                            <span id="Label212" data-type="SheetLabel" data-datafield="DbMaritalStatusName" style="">婚姻状况</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control220" class="rightbox2">
                            <input id="Control212" type="text" data-datafield="DbMaritalStatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">
                            第三方外部信用记录
                        </div>
                        <div class="centerline3"></div>
                        <div class="rightbox3" id="dbrwbxy">
                            <span>查询中</span>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title279" class="leftbox1">
                            <span id="Label265" data-type="SheetLabel" data-datafield="DbcurrentLivingAddress" style="">地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control279" class="rightbox1">
                            <textarea id="Control265" data-datafield="DbcurrentLivingAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title286" class="leftbox3">
                            <span id="Label270" data-type="SheetLabel" data-datafield="Dbnativedistrict" style="">担保人籍贯</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control286" class="rightbox3">
                            <input id="Control270" type="text" data-datafield="Dbnativedistrict" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title281" class="leftbox1">
                            <span id="Label266" data-type="SheetLabel" data-datafield="DbAddressId" style="">担保人户籍地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control281" class="rightbox1">
                            <textarea id="Control266" data-datafield="DbAddressId" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title288" class="leftbox3">
                            <span id="Label272" data-type="SheetLabel" data-datafield="DbhukouprovinceName" style="">担保人省份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control288" class="rightbox3">
                            <input id="Control272" type="text" data-datafield="DbhukouprovinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title261" class="leftbox1">
                            <span id="Label250" data-type="SheetLabel" data-datafield="DbCompanyName" style="">公司名称</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control261" class="rightbox1">
                            <input id="Control250" data-datafield="DbCompanyName" data-type="SheetTextBox" style="" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title263" class="leftbox3">
                            <span id="Label251" data-type="SheetLabel" data-datafield="DbBusinessTypeName" style="">企业性质</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control263" class="rightbox3">
                            <input id="Control251" type="text" data-datafield="DbBusinessTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title267" class="leftbox">
                            <span id="Label255" data-type="SheetLabel" data-datafield="Dbphonenumber" style="">公司电话 </span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control267" class="rightbox">
                            <input id="Control255" type="text" data-datafield="Dbphonenumber" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title275" class="leftbox2">
                            <span id="Label262" data-type="SheetLabel" data-datafield="Dbtimeinyear" style="">工作年限（年）</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control275" class="rightbox2">
                            <input id="Control262" type="text" data-datafield="Dbtimeinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title259" class="leftbox3">
                            <span id="Label249" data-type="SheetLabel" data-datafield="DbActualSalary" style="">月收入</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control259" class="rightbox3">
                            <input id="Control249" type="text" data-datafield="DbActualSalary" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div id="divDBR1">
                <div class="row">
                    <div class="col-md-4">
                        <div id="title218" class="leftbox">
                            <span id="Label210" data-type="SheetLabel" data-datafield="DbIdCarTypeName" style="">担保人证件类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control218" class="rightbox">
                            <input id="Control210" type="text" data-datafield="DbIdCarTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title221" class="leftbox2">
                            <span id="Label213" data-type="SheetLabel" data-datafield="DbgenderName" style="">担保人性别</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control221" class="rightbox2">
                            <input id="Control213" type="text" data-datafield="DbgenderName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title226" class="leftbox3">
                            <span id="Label216" data-type="SheetLabel" data-datafield="DbCitizenshipName" style="">担保人国籍</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control226" class="rightbox3">
                            <input id="Control216" type="text" data-datafield="DbCitizenshipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title227" class="leftbox">
                            <span id="Label217" data-type="SheetLabel" data-datafield="DbIdCardIssueDate" style="">担保人证件发行日</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control227" class="rightbox">
                            <input id="Control217" type="text" data-datafield="DbIdCardIssueDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title228" class="leftbox2">
                            <span id="Label218" data-type="SheetLabel" data-datafield="DbIdCardExpiryDate" style="">担保人证件到期日</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control228" class="rightbox2">
                            <input id="Control218" type="text" data-datafield="DbIdCardExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title229" class="leftbox3">
                            <span id="Label219" data-type="SheetLabel" data-datafield="DbLicenseNo" style="">担保人驾照号码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control229" class="rightbox3">
                            <input id="Control219" type="text" data-datafield="DbLicenseNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title230" class="leftbox">
                            <span id="Label220" data-type="SheetLabel" data-datafield="DbLicenseExpiryDate" style="">担保人驾照到期日</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control230" class="rightbox">
                            <input id="Control220" type="text" data-datafield="DbLicenseExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title231" class="leftbox2">
                            <span id="Label221" data-type="SheetLabel" data-datafield="DbThaiTitleName" style="">担保人头衔</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control231" class="rightbox2">
                            <input id="Control221" type="text" data-datafield="DbThaiTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title232" class="leftbox3">
                            <span id="Label222" data-type="SheetLabel" data-datafield="DbTitleName" style="">担保人头衔（英文）</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control232" class="rightbox3">
                            <input id="Control222" type="text" data-datafield="DbTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title234" class="leftbox">
                            <span id="Label224" data-type="SheetLabel" data-datafield="DbEngFirstName" style="">担保人名（英文）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control234" class="rightbox">
                            <input id="Control224" type="text" data-datafield="DbEngFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title235" class="leftbox2">
                            <span id="Label225" data-type="SheetLabel" data-datafield="DbEngLastName" style="">担保人中间名字</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control235" class="rightbox2">
                            <input id="Control225" type="text" data-datafield="DbEngLastName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title236" class="leftbox3">
                            <span id="Label226" data-type="SheetLabel" data-datafield="DbFormerName" style="">担保人曾用名</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control236" class="rightbox3">
                            <input id="Control226" type="text" data-datafield="DbFormerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title237" class="leftbox">
                            <span id="Label227" data-type="SheetLabel" data-datafield="DbNationCode" style="">担保人民族</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control237" class="rightbox">
                            <input id="Control227" type="text" data-datafield="DbNationCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title238" class="leftbox2">
                            <span id="Label228" data-type="SheetLabel" data-datafield="DbIssuingAuthority" style="">担保人签发机关</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control238" class="rightbox2">
                            <input id="Control228" type="text" data-datafield="DbIssuingAuthority" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title239" class="leftbox3">
                            <span id="Label229" data-type="SheetLabel" data-datafield="DbNumberOfDependents" style="">担保人供养人数</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control239" class="rightbox3">
                            <input id="Control229" type="text" data-datafield="DbNumberOfDependents" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title240" class="leftbox">
                            <span id="Label230" data-type="SheetLabel" data-datafield="DbHouseOwnerName" style="">担保人房产所有人</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control240" class="rightbox">
                            <input id="Control230" type="text" data-datafield="DbHouseOwnerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title241" class="leftbox2">
                            <span id="Label231" data-type="SheetLabel" data-datafield="DbNoOfFamilyMembers" style="">担保人家庭人数</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control241" class="rightbox2">
                            <input id="Control231" type="text" data-datafield="DbNoOfFamilyMembers" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title242" class="leftbox3">
                            <span id="Label232" data-type="SheetLabel" data-datafield="DbChildrenFlag" style="">担保人是否是子女</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control242" class="rightbox3">
                            <input id="Control232" type="text" data-datafield="DbChildrenFlag" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title243" class="leftbox">
                            <span id="Label233" data-type="SheetLabel" data-datafield="DbEmploymentTypeName" style="">担保人雇员类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control243" class="rightbox">
                            <input id="Control233" type="text" data-datafield="DbEmploymentTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title244" class="leftbox2">
                            <span id="Label234" data-type="SheetLabel" data-datafield="DbEmailAddress" style="">担保人邮箱地址</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control244" class="rightbox2">
                            <input id="Control234" type="text" data-datafield="DbEmailAddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title246" class="leftbox3">
                            <span id="Label236" data-type="SheetLabel" data-datafield="DbIndustryTypeName" style="">担保人行业类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control246" class="rightbox3">
                            <input id="Control236" type="text" data-datafield="DbIndustryTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title247" class="leftbox">
                            <span id="Label237" data-type="SheetLabel" data-datafield="DbIndustrySubTypeName" style="">担保人行业子类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control247" class="rightbox">
                            <input id="Control237" type="text" data-datafield="DbIndustrySubTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title248" class="leftbox2">
                            <span id="Label238" data-type="SheetLabel" data-datafield="DbOccupationName" style="">担保人职业类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control248" class="rightbox2">
                            <input id="Control238" type="text" data-datafield="DbOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title249" class="leftbox3">
                            <span id="Label239" data-type="SheetLabel" data-datafield="DbSubOccupationName" style="">担保人职业子类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control249" class="rightbox3">
                            <input id="Control239" type="text" data-datafield="DbSubOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title250" class="leftbox">
                            <span id="Label240" data-type="SheetLabel" data-datafield="DbDesginationName" style="">担保人职位</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control250" class="rightbox">
                            <input id="Control240" type="text" data-datafield="DbDesginationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title251" class="leftbox2">
                            <span id="Label241" data-type="SheetLabel" data-datafield="DbJobGroupName" style="">担保人工作组</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control251" class="rightbox2">
                            <input id="Control241" type="text" data-datafield="DbJobGroupName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title252" class="leftbox3">
                            <span id="Label242" data-type="SheetLabel" data-datafield="DbSalaryRangeName" style="">担保人估计收入</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control252" class="rightbox3">
                            <input id="Control242" type="text" data-datafield="DbSalaryRangeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title253" class="leftbox">
                            <span id="Label243" data-type="SheetLabel" data-datafield="DbConsent" style="">担保人同意</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control253" class="rightbox">
                            <input id="Control243" type="text" data-datafield="DbConsent" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title254" class="leftbox2">
                            <span id="Label244" data-type="SheetLabel" data-datafield="DbVIPInd" style="">担保人贵宾</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control254" class="rightbox2">
                            <input id="Control244" type="text" data-datafield="DbVIPInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title255" class="leftbox3">
                            <span id="Label245" data-type="SheetLabel" data-datafield="DbStaffInd" style="">担保人工作人员</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control255" class="rightbox3">
                            <input id="Control245" type="text" data-datafield="DbStaffInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title256" class="leftbox">
                            <span id="Label246" data-type="SheetLabel" data-datafield="DbBlackListNoRecordInd" style="">担保人黑名单无记录</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control256" class="rightbox">
                            <input id="Control246" type="text" data-datafield="DbBlackListNoRecordInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title257" class="leftbox2">
                            <span id="Label247" data-type="SheetLabel" data-datafield="DbBlacklistInd" style="">担保人外部信用记录</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control257" class="rightbox2">
                            <input id="Control247" type="text" data-datafield="DbBlacklistInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title264" class="leftbox3">
                            <span id="Label252" data-type="SheetLabel" data-datafield="DbpositionName" style="">担保人职位</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control264" class="rightbox3">
                            <input id="Control252" type="text" data-datafield="DbpositionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title265" class="leftbox">
                            <span id="Label253" data-type="SheetLabel" data-datafield="DbcompanyProvinceName" style="">担保人公司省份</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control265" class="rightbox">
                            <input id="Control253" type="text" data-datafield="DbcompanyProvinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title268" class="leftbox2">
                            <span id="Label256" data-type="SheetLabel" data-datafield="DbFax" style="">担保人公司传真 </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control268" class="rightbox2">
                            <input id="Control256" type="text" data-datafield="DbFax" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title271" class="leftbox3">
                            <span id="Label258" data-type="SheetLabel" data-datafield="DbcompanypostCode" style="">担保人公司邮编 </span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control271" class="rightbox3">
                            <input id="Control258" type="text" data-datafield="DbcompanypostCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title269" class="leftbox1">
                            <span id="Label257" data-type="SheetLabel" data-datafield="DbcompanyAddress" style="">担保人公司地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control269" class="rightbox1">
                            <textarea id="Control257" data-datafield="DbcompanyAddress" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title272" class="leftbox3">
                            <span id="Label259" data-type="SheetLabel" data-datafield="DbEmployerType" style="">担保人雇主类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control272" class="rightbox3">
                            <input id="Control259" type="text" data-datafield="DbEmployerType" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title273" class="leftbox">
                            <span id="Label260" data-type="SheetLabel" data-datafield="DbNoOfEmployees" style="">担保人雇员人数</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control273" class="rightbox">
                            <input id="Control260" type="text" data-datafield="DbNoOfEmployees" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title274" class="leftbox2">
                            <span id="Label261" data-type="SheetLabel" data-datafield="DbJobDescription" style="">担保人工作描述</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control274" class="rightbox2">
                            <input id="Control261" type="text" data-datafield="DbJobDescription" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title276" class="leftbox3">
                            <span id="Label263" data-type="SheetLabel" data-datafield="Dbtimeinmonth" style="">担保人工作年限（月） </span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control276" class="rightbox3">
                            <input id="Control263" type="text" data-datafield="Dbtimeinmonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div class="col-md-8">
                        <div id="title277" class="leftbox1">
                            <span id="Label264" data-type="SheetLabel" data-datafield="DbComments" style="">担保人公司评论</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control277" class="rightbox1">
                            <textarea id="Control264" data-datafield="DbComments" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title283" class="leftbox3">
                            <span id="Label267" data-type="SheetLabel" data-datafield="Dbdefaultmailingaddress" style="">担保人默认邮寄地址</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control283" class="rightbox3">
                            <input id="Control267" type="text" data-datafield="Dbdefaultmailingaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title284" class="leftbox">
                            <span id="Label268" data-type="SheetLabel" data-datafield="Dbhukouaddress" style="">担保人户籍地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control284" class="rightbox">
                            <input id="Control268" type="text" data-datafield="Dbhukouaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title285" class="leftbox2">
                            <span id="Label269" data-type="SheetLabel" data-datafield="DbcountryName" style="">担保人国家</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control285" class="rightbox2">
                            <input id="Control269" type="text" data-datafield="DbcountryName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title287" class="leftbox3">
                            <span id="Label271" data-type="SheetLabel" data-datafield="Dbbirthpalaceprovince" style="">担保人出生地省市县（区） </span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control287" class="rightbox3">
                            <input id="Control271" type="text" data-datafield="Dbbirthpalaceprovince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title289" class="leftbox">
                            <span id="Label273" data-type="SheetLabel" data-datafield="DbhukoucityName" style="">担保人城市</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control289" class="rightbox">
                            <input id="Control273" type="text" data-datafield="DbhukoucityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title290" class="leftbox2">
                            <span id="Label274" data-type="SheetLabel" data-datafield="Dbpostcode" style="">担保人邮编</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control290" class="rightbox2">
                            <input id="Control274" type="text" data-datafield="Dbpostcode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title291" class="leftbox3">
                            <span id="Label275" data-type="SheetLabel" data-datafield="DbaddresstypeName" style="">担保人地址类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control291" class="rightbox3">
                            <input id="Control275" type="text" data-datafield="DbaddresstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title292" class="leftbox">
                            <span id="Label276" data-type="SheetLabel" data-datafield="DbaddressstatusName" style="">担保人地址状态</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control292" class="rightbox">
                            <input id="Control276" type="text" data-datafield="DbaddressstatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title293" class="leftbox2">
                            <span id="Label277" data-type="SheetLabel" data-datafield="DbpropertytypeName" style="">担保人房产类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control293" class="rightbox2">
                            <input id="Control277" type="text" data-datafield="DbpropertytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title294" class="leftbox3">
                            <span id="Label278" data-type="SheetLabel" data-datafield="DbresidencetypeName" style="">担保人住宅类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control294" class="rightbox3">
                            <input id="Control278" type="text" data-datafield="DbresidencetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title295" class="leftbox">
                            <span id="Label279" data-type="SheetLabel" data-datafield="Dblivingsince" style="">担保人开始居住日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control295" class="rightbox">
                            <input id="Control279" type="text" data-datafield="Dblivingsince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title296" class="leftbox2">
                            <span id="Label280" data-type="SheetLabel" data-datafield="Dbhomevalue" style="">担保人房屋价值（万元) </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control296" class="rightbox2">
                            <input id="Control280" type="text" data-datafield="Dbhomevalue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title297" class="leftbox3">
                            <span id="Label281" data-type="SheetLabel" data-datafield="Dbstayinyear" style="">担保人居住年限</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control297" class="rightbox3">
                            <input id="Control281" type="text" data-datafield="Dbstayinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title298" class="leftbox">
                            <span id="Label282" data-type="SheetLabel" data-datafield="Dbstayinmonth" style="">担保人居住月份</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control298" class="rightbox">
                            <input id="Control282" type="text" data-datafield="Dbstayinmonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title299" class="leftbox2">
                            <span id="Label283" data-type="SheetLabel" data-datafield="DbcountrycodeName" style="">担保人国家代码</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control299" class="rightbox2">
                            <input id="Control283" type="text" data-datafield="DbcountrycodeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title300" class="leftbox3">
                            <span id="Label284" data-type="SheetLabel" data-datafield="DbareaCode" style="">担保人地区代码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control300" class="rightbox3">
                            <input id="Control284" type="text" data-datafield="DbareaCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title302" class="leftbox">
                            <span id="Label286" data-type="SheetLabel" data-datafield="Dbextension" style="">担保人分机</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control302" class="rightbox">
                            <input id="Control286" type="text" data-datafield="Dbextension" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title303" class="leftbox2">
                            <span id="Label287" data-type="SheetLabel" data-datafield="DbphonetypeName" style="">担保人电话类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control303" class="rightbox2">
                            <input id="Control287" type="text" data-datafield="DbphonetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title266" class="leftbox3">
                            <span id="Label254" data-type="SheetLabel" data-datafield="DbcompanyCityName" style="">单位城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control266" class="rightbox3">
                            <input id="Control254" type="text" data-datafield="DbcompanyCityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                <a href="javascript:void(0);" onclick="hideInfo('divDBR1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label id="Label3" data-en_us="Basic information">资产信息</label>
        </div>
        <div class="divContent" id="divZC">
            <div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title304" class="leftbox ">
                            <span id="Label288" data-type="SheetLabel" data-datafield="assetConditionName" style="">资产状况</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control304" class="rightbox ">
                            <input id="Control288" type="text" data-datafield="assetConditionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title305" class="leftbox2">
                            <span id="Label289" data-type="SheetLabel" data-datafield="assetMakeName" style="">制造商</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control305" class="rightbox2">
                            <input id="Control289" type="text" data-datafield="assetMakeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title306" class="leftbox3">
                            <span id="Label290" data-type="SheetLabel" data-datafield="brandName" style="">车型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control306" class="rightbox3">
                            <input id="Control290" type="text" data-datafield="brandName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title309" class="leftbox">
                            <span id="Label293" data-type="SheetLabel" data-datafield="assetPrice" style="">新车指导价</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control309" class="rightbox ">
                            <input id="Control293" type="text" data-datafield="assetPrice" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title310" class="leftbox2">
                            <span id="Label294" data-type="SheetLabel" data-datafield="transmissionName" style="">变速器</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control310" class="rightbox2">
                            <input id="Control294" type="text" data-datafield="transmissionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title319" class="leftbox3">
                            <span id="Label303" data-type="SheetLabel" data-datafield="vehicleAge" style="">车辆使用年数</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control319" class="rightbox3">
                            <input id="Control303" type="text" data-datafield="vehicleAge" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title321" class="leftbox">
                            <span id="Label305" data-type="SheetLabel" data-datafield="releaseDate" style="">出厂日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control321" class="rightbox">
                            <input id="Control305" type="text" data-datafield="releaseDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title329" class="leftbox2">
                            <span id="Label313" data-type="SheetLabel" data-datafield="odometer" style="">里程数</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control329" class="rightbox2">
                            <input id="Control313" type="text" data-datafield="odometer" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title312" class="leftbox3">
                            <span id="Label296" data-type="SheetLabel" data-datafield="purposeName" style="">购车目的</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control312" class="rightbox3">
                            <input id="Control296" type="text" data-datafield="purposeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div class="col-md-8">
                        <div id="title333" class="leftbox1">
                            <span id="Label316" data-type="SheetLabel" data-datafield="vecomments" style="">车辆备注</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control333" class="rightbox1">
                            <textarea id="Control316" data-datafield="vecomments" data-type="SheetRichTextBox" style="">					</textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divZC1">
                <div class="row">
                    <div class="col-md-4">
                        <div id="title307" class="leftbox">
                            <span id="Label291" data-type="SheetLabel" data-datafield="modelName" style="">动力参数</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control307" class="rightbox">
                            <input id="Control291" type="text" data-datafield="modelName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title308" class="leftbox2">
                            <span id="Label292" data-type="SheetLabel" data-datafield="MIOCN" style="">MIOCN</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control308" class="rightbox2">
                            <input id="Control292" type="text" data-datafield="MIOCN" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title311" class="leftbox3">
                            <span id="Label295" data-type="SheetLabel" data-datafield="vehicleSubtypeName" style="">颜色</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control311" class="rightbox3">
                            <input id="Control295" type="text" data-datafield="vehicleSubtypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title313" class="leftbox">
                            <span id="Label297" data-type="SheetLabel" data-datafield="engineNo" style="">发动机号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control313" class="rightbox">
                            <input id="Control297" type="text" data-datafield="engineNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title314" class="leftbox2">
                            <span id="Label298" data-type="SheetLabel" data-datafield="vinNo" style="">VIN号</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control314" class="rightbox2">
                            <input id="Control298" type="text" data-datafield="vinNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title315" class="leftbox3">
                            <span id="Label299" data-type="SheetLabel" data-datafield="vehiclecolorName" style="">车身颜色</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control315" class="rightbox3">
                            <input id="Control299" type="text" data-datafield="vehiclecolorName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title316" class="leftbox">
                            <span id="Label300" data-type="SheetLabel" data-datafield="vineditind" style="">编辑VIN号</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control316" class="rightbox">
                            <input id="Control300" type="text" data-datafield="vineditind" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title317" class="leftbox2">
                            <span id="Label301" data-type="SheetLabel" data-datafield="manufactureYear" style="">Manufacture Year </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control317" class="rightbox2">
                            <input id="Control301" type="text" data-datafield="manufactureYear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title318" class="leftbox3">
                            <span id="Label302" data-type="SheetLabel" data-datafield="engineCC" style="">发动机排量</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control318" class="rightbox3">
                            <input id="Control302" type="text" data-datafield="engineCC" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title320" class="leftbox">
                            <span id="Label304" data-type="SheetLabel" data-datafield="deliveryDate" style="">TBR日期</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control320" class="rightbox">
                            <input id="Control304" type="text" data-datafield="deliveryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title322" class="leftbox2">
                            <span id="Label306" data-type="SheetLabel" data-datafield="releaseMonth" style="">出厂月份</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control322" class="rightbox2">
                            <input id="Control306" type="text" data-datafield="releaseMonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title323" class="leftbox3">
                            <span id="Label307" data-type="SheetLabel" data-datafield="releaseYear" style="">出厂年份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control323" class="rightbox3">
                            <input id="Control307" type="text" data-datafield="releaseYear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title324" class="leftbox">
                            <span id="Label308" data-type="SheetLabel" data-datafield="registrationNo" style="">注册号</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control324" class="rightbox">
                            <input id="Control308" type="text" data-datafield="registrationNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title325" class="leftbox2">
                            <span id="Label309" data-type="SheetLabel" data-datafield="series" style="">系列</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control325" class="rightbox2">
                            <input id="Control309" type="text" data-datafield="series" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title326" class="leftbox3">
                            <span id="Label310" data-type="SheetLabel" data-datafield="vehicleBody" style="">车身</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control326" class="rightbox3">
                            <input id="Control310" type="text" data-datafield="vehicleBody" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title327" class="leftbox">
                            <span id="Label311" data-type="SheetLabel" data-datafield="style" style="">风格</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control327" class="rightbox">
                            <input id="Control311" type="text" data-datafield="style" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title328" class="leftbox2">
                            <span id="Label312" data-type="SheetLabel" data-datafield="cylinder" style="">汽缸</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control328" class="rightbox2">
                            <input id="Control312" type="text" data-datafield="cylinder" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                <a href="javascript:void(0);" onclick="hideInfo('divZC1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label id="Label4" data-en_us="Basic information">金融条款</label>
        </div>
        <div class="divContent" id="divJRTK" style="padding-right: 0;">
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 90%; margin-bottom: 10px; border-bottom: 1px solid #ccc;">
                <label data-en_us="Sheet information">产品选择</label>
            </div>
            <div style="position: relative; padding-right: 10%;" id="divCPXZ">
                <div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title335" class="leftbox">
                                <span id="Label317" data-type="SheetLabel" data-datafield="productGroupName" style="">产品组</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control335" class="rightbox">
                                <input id="Control317" type="text" data-datafield="productGroupName" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title336" class="leftbox2">
                                <span id="Label318" data-type="SheetLabel" data-datafield="productTypeName" style="">产品类型</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control336" class="rightbox2">
                                <input id="Control318" type="text" data-datafield="productTypeName" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title337" class="leftbox3">
                                <span id="Label319" data-type="SheetLabel" data-datafield="paymentFrequencyName" style="">付款频率</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control337" class="rightbox3">
                                <input id="Control319" type="text" data-datafield="paymentFrequencyName" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title339" class="leftbox">
                                <span id="Label321" data-type="SheetLabel" data-datafield="termMonth" style="">贷款期数（月）</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control339" class="rightbox">
                                <input id="Control321" type="text" data-datafield="termMonth" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title330" class="leftbox2">
                                <span id="Label314" data-type="SheetLabel" data-datafield="totalaccessoryamount" style="">附加费</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control330" class="rightbox2">
                                <input id="Control314" type="text" data-datafield="totalaccessoryamount" data-type="SheetTextBox" style="color:red;">
                                <a href="#accessoryDetail" data-toggle="modal" onclick="" target="_blank">详细</a>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title3571" class="leftbox3">
                                <span id="Label3391" data-type="SheetLabel" data-datafield="ckje" style="">敞口金额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control3571" class="rightbox3">
                                <span id="ctrlCkje" data-type="SheetLabel" data-datafield="ckje" style="color: red;"></span>
                                <a href="#example" data-toggle="modal" onclick="" target="_blank">详细</a>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title340" class="leftbox">
                                <span id="Label322" data-type="SheetLabel" data-datafield="salesprice" style="">合同价格</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control340" class="rightbox">
                                <input id="Control322" type="text" data-datafield="salesprice" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title344" class="leftbox2">
                                <span id="Label326" data-type="SheetLabel" data-datafield="downpaymentrate" style="">首付款比例 %</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control344" class="rightbox2">
                                <input id="Control326" type="text" data-datafield="downpaymentrate" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title346" class="leftbox3">
                                <span id="Label328" data-type="SheetLabel" data-datafield="downpaymentamount" style="">首付款金额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control346" class="rightbox3">
                                <input id="Control328" type="text" data-datafield="downpaymentamount" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title342" class="leftbox">
                                <span id="Label324" data-type="SheetLabel" data-datafield="vehicleprice" style="">销售价格</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control342" class="rightbox">
                                <input id="Control324" type="text" data-datafield="vehicleprice" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title350" class="leftbox2">
                                <span id="Label332" data-type="SheetLabel" data-datafield="financedamountrate" style="">贷款额比例%</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control350" class="rightbox2">
                                <input id="Control332" type="text" data-datafield="financedamountrate" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title348" class="leftbox3">
                                <span id="Label330" data-type="SheetLabel" data-datafield="financedamount" style="">贷款金额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control348" class="rightbox3">
                                <input id="ctrlfinancedamount" type="text" data-datafield="financedamount" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title359" class="leftbox1" style="width: 11.666666%;">
                            <span id="Label341" data-type="SheetLabel" data-datafield="RentalDetailtable" style="">还款计划</span>
                        </div>
                        <div id="control359" class="rightbox1">
                            <table id="Table1" data-datafield="RentalDetailtable" data-type="SheetGridView" class="SheetGridView">
                                <tbody>
                                    <tr class="header">
                                        <td id="Control341_SerialNo" class="rowSerialNo">序号</td>
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
                                        <td class="rowOption">删除</td>
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
                </div>
                <div id="divJRTK1">
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title338" class="leftbox">
                                <span id="Label320" data-type="SheetLabel" data-datafield="rentalmodeName" style="">租赁模式</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control338" class="rightbox">
                                <input id="Control320" type="text" data-datafield="rentalmodeName" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title341" class="leftbox2">
                                <span id="Label323" data-type="SheetLabel" data-datafield="deferredTerm" style="">展期期数</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control341" class="rightbox2">
                                <input id="Control323" type="text" data-datafield="deferredTerm" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title343" class="leftbox3">
                                <span id="Label325" data-type="SheetLabel" data-datafield="balloonRate" style="">资产残值/轻松融资尾款%</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control343" class="rightbox3">
                                <input id="Control325" type="text" data-datafield="balloonRate" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title345" class="leftbox">
                                <span id="Label327" data-type="SheetLabel" data-datafield="balloonAmount" style="">资产残值/轻松融资尾款金额</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control345" class="rightbox">
                                <input id="Control327" type="text" data-datafield="balloonAmount" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title347" class="leftbox2">
                                <span id="Label329" data-type="SheetLabel" data-datafield="interestRate" style="">客户利率% </span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control347" class="rightbox2">
                                <input id="Control329" type="text" data-datafield="interestRate" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title349" class="leftbox3">
                                <span id="Label331" data-type="SheetLabel" data-datafield="subsidyAmount" style="">贴息金额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control349" class="rightbox3">
                                <input id="Control331" type="text" data-datafield="subsidyAmount" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title351" class="leftbox">
                                <span id="Label333" data-type="SheetLabel" data-datafield="otherfees" style="">其他费用</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control351" class="rightbox">
                                <input id="Control333" type="text" data-datafield="otherfees" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title352" class="leftbox2">
                                <span id="Label334" data-type="SheetLabel" data-datafield="actualRate" style="">实际利率</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control352" class="rightbox2">
                                <input id="Control334" type="text" data-datafield="actualRate" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title353" class="leftbox3">
                                <span id="Label335" data-type="SheetLabel" data-datafield="totalintamount" style="">利息总额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control353" class="rightbox3">
                                <input id="Control335" type="text" data-datafield="totalintamount" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title354" class="leftbox">
                                <span id="Label336" data-type="SheetLabel" data-datafield="subsidyRate" style="">贴息利率</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control354" class="rightbox">
                                <input id="Control336" type="text" data-datafield="subsidyRate" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title355" class="leftbox2">
                                <span id="Label337" data-type="SheetLabel" data-datafield="totalamountpayable" style="">应付总额</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control355" class="rightbox2">
                                <input id="Control337" type="text" data-datafield="totalamountpayable" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title356" class="leftbox3">
                                <span id="Label338" data-type="SheetLabel" data-datafield="additionalName" style="">是否有附加费</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control356" class="rightbox3">
                                <input id="Control338" type="text" data-datafield="additionalName" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title357" class="leftbox">
                                <span id="Label339" data-type="SheetLabel" data-datafield="price" style="">价格</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control357" class="rightbox">
                                <input id="Control339" type="text" data-datafield="price" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title358" class="leftbox2">
                                <span id="Label340" data-type="SheetLabel" data-datafield="BalancePayable" style="">未偿余额</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control358" class="rightbox2">
                                <input id="Control340" type="text" data-datafield="BalancePayable" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title407" class="leftbox3">
                                <span id="Label366" data-type="SheetLabel" data-datafield="Bankname" style="">客户银行名称</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control407" class="rightbox3">
                                <input data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title408" class="leftbox">
                                <span id="Label367" data-type="SheetLabel" data-datafield="Branchname" style="">客户银行分支</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control408" class="rightbox">
                                <input data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title409" class="leftbox2">
                                <span id="Label368" data-type="SheetLabel" data-datafield="Accoutname" style="">客户账户名</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control409" class="rightbox2">
                                <input id="Control368" type="text" data-datafield="Accoutname" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title410" class="leftbox3">
                                <span id="Label369" data-type="SheetLabel" oninput="textnum(this)" data-datafield="AccoutNumber" style="">客户账户号</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control410" class="rightbox3">
                                <input id="Control369" type="text" data-datafield="AccoutNumber" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                </div>
                <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                    <a href="javascript:void(0);" onclick="hideInfo('divJRTK1',this)">收起 &and;</a>
                </div>
            </div>
            <%-------------------客户测算-------------------------------%>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 90%; margin-top: 10px; margin-bottom: 10px; border-bottom: 1px solid #ccc;">
                <label data-en_us="Sheet information">客户测算</label>
            </div>
            <div style="position: relative; padding-right: 10%;" id="divCustoms">
                <div>
                      <div class="row tableContent">
                        <div id="titlesqr" class="leftbox1" style="width: 11.666666%;">
                            <span id="Labesqr" data-type="SheetLabel"  style="">申请人</span>
                        </div>
                        <div id="controsqrr" class="rightbox1">
                            <table id="TableCus"  data-type="SheetGridView" class="SheetGridView">
                                <tbody>
                                    <tr class="header">
                                        <td id="ControIncomOfMonth_Header" >
                                            <label id="ControIncomOfMonth_Label"  data-type="SheetLabel" style="">月收入估值</label>
                                        </td>
                                        <td id="ControDabtsOfMonth_Heade">
                                            <label id="ControDabtsOfMonth_Label"  data-type="SheetLabel" style="">月应还债务</label>
                                        </td>
                                        <td id="ControAssetValuation_Header">
                                            <label id="ControAssetValuation_Label" data-type="SheetLabel" style="">客户资产估值</label>
                                        </td>
                                        <td id="ControRepayLoan_Header" >
                                            <label id="ControRepayLoan_Label" data-type="SheetLabel" style="">客户月还贷能力</label>
                                        </td>
                                    </tr>
                                    <tr class="">
                                        <td >
                                            <input id="ControIncomOfMonth_ctl" type="text"  data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                        <td >
                                            <input id="ControDabtsOfMonth_ctl" type="text"  data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                        <td>
                                            <input id="ControAssetValuation_ctl" type="text"  data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                        <td >
                                            <input id="ControRepayLoan_ctl" type="text" data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                    </tr>
                                    
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div id="divgGGustom">
                     <div class="row tableContent">
                        <div id="titlegsqr" class="leftbox1" style="width: 11.666666%;">
                            <span id="Labegsqr" data-type="SheetLabel"  style="">共借人</span>
                        </div>
                        <div id="controgsqrr" class="rightbox1">
                            <table id="TablegCus"  data-type="SheetGridView" class="SheetGridView">
                                <tbody>
                                    <tr class="header">
                                        <td id="ControgIncomOfMonth_Header" >
                                            <label id="ControgIncomOfMonth_Label"  data-type="SheetLabel" style="">月收入估值</label>
                                        </td>
                                        <td id="ControgDabtsOfMonth_Heade">
                                            <label id="ControgDabtsOfMonth_Label"  data-type="SheetLabel" style="">月应还债务</label>
                                        </td>
                                        <td id="ControgAssetValuation_Header">
                                            <label id="ControgAssetValuation_Label" data-type="SheetLabel" style="">客户资产估值</label>
                                        </td>
                                        <td id="ControgRepayLoan_Header" >
                                            <label id="ControgRepayLoan_Label" data-type="SheetLabel" style="">客户月还贷能力</label>
                                        </td>
                                    </tr>
                                    <tr class="">
                                        <td >
                                            <input id="ControgIncomOfMonth_ctl" type="text"  data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                        <td >
                                            <input id="ControgDabtsOfMonth_ctl" type="text"  data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                        <td>
                                            <input id="ControgAssetValuation_ctl" type="text"  data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                        <td >
                                            <input id="ControgRepayLoan_ctl" type="text" data-type="SheetTextBox" style="text-align:center">
                                        </td>
                                    </tr>
                                    
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div data-isretract="true" id="divgLine" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                    <a href="javascript:void(0);" onclick="hideInfo('divgGGustom',this)">收起 &and;</a>
                </div>
            </div>
            <%--------------------------------------------------%>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 90%; margin-top: 10px; margin-bottom: 10px; border-bottom: 1px solid #ccc;">
                <label data-en_us="Sheet information">资产/负债</label>
            </div>
            <div style="position: relative; padding-right: 10%;" id="divZCFZ">
                <div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title429" class="leftbox">
                                <span id="Label384" data-type="SheetLabel" data-datafield="Dfgz" style="">代发工资</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control429" class="rightbox">
                                <input id="Control384" type="text" data-datafield="Dfgz" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title432" class="leftbox2">
                                <span id="Label387" data-type="SheetLabel" data-datafield="Hqjx" style="">活期结息</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control432" class="rightbox2">
                                <input id="Control387" type="text" data-datafield="Hqjx" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title437" class="leftbox3">
                                <span id="Label392" data-type="SheetLabel" data-datafield="HQRJ" style="">活期日均</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control437" class="rightbox3">
                                <input id="Control392" type="text" data-datafield="HQRJ" data-type="SheetTextBox" style="" class="" data-computationrule="2,{Hqjx}*4/0.0035">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title435" class="leftbox">
                                <span id="Label390" data-type="SheetLabel" data-datafield="Bccdygje" style="">本次车贷月供金额</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control435" class="rightbox">
                                <input id="Control390" type="text" data-datafield="Bccdygje" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title436" class="leftbox2">
                                <span id="Label391" data-type="SheetLabel" data-datafield="QTFZYG" style="">其他负债月供</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control436" class="rightbox2">
                                <input id="Text1" type="text" data-datafield="QTFZYG" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title438" class="leftbox3">
                                <span id="Label393" data-type="SheetLabel" data-datafield="JRZC" style="">金融资产</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control438" class="rightbox3">
                                <input id="Control393" type="text" data-datafield="JRZC" data-type="SheetTextBox" style="" data-computationrule="2,{Hqjx}*4/0.0035*0.7 +{Dqck} *0.7 +{Lc}*0.5 + {Gp} * 0.4 + {Gj}*0.5">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title439" class="leftbox ">
                                <span id="Label394" data-type="SheetLabel" data-datafield="SRFZB" style="">收入负债比</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control439" class="rightbox " style="color: red;">
                                <input id="Control394" type="text" data-datafield="SRFZB" data-type="SheetTextBox" style="" data-computationrule="2,({Bccdygje}+{QTFZYG})/({Dfgz}*1.2+({Hqjx}*4/0.0035*0.7 + {Dqck} *0.7 + {Lc}*0.5 + {Gp} * 0.4 + {Gj}*0.5)/2)">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title111" class="leftbox2">
                                <span id="Label111" data-type="SheetLabel" data-datafield="Financialratiodes" style="">财务比率</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control111" class="rightbox2">
                                <input id="Control111" type="text" data-datafield="Financialratiodes" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title112" class="leftbox3">
                                <span id="Label112" data-type="SheetLabel" data-datafield="Evalresult" style="">财务比率金额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control112" class="rightbox3">
                                <input id="Control112" type="text" data-datafield="Evalresult" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                </div>
                <div id="divZCFZ1">
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title108" class="leftbox">
                                <span id="Label108" data-type="SheetLabel" data-datafield="Descritption" style="">收入</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control108" class="rightbox">
                                <input id="Control108" type="text" data-datafield="Descritption" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title430" class="leftbox2">
                                <span id="Label385" data-type="SheetLabel" data-datafield="Dqck" style="">定期存款</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control430" class="rightbox2">
                                <input id="Control385" type="text" data-datafield="Dqck" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title431" class="leftbox3">
                                <span id="Label386" data-type="SheetLabel" data-datafield="Gp" style="">股票</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="control431" class="rightbox3">
                                <input id="Control386" type="text" data-datafield="Gp" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div id="title433" class="leftbox">
                                <span id="Label388" data-type="SheetLabel" data-datafield="Gj" style="">基金</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control433" class="rightbox">
                                <input id="Control388" type="text" data-datafield="Gj" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title434" class="leftbox2">
                                <span id="Label389" data-type="SheetLabel" data-datafield="Lc" style="">理财</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control434" class="rightbox2">
                                <input id="Control389" type="text" data-datafield="Lc" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                </div>
                <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                    <a href="javascript:void(0);" onclick="hideInfo('divZCFZ1',this)">收起 &and;</a>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divattachment',this)">
            <label id="Label1" data-en_us="Sheet information">附件信息</label>
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
                    <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" style=""></div>
                </div>
            </div>
            <div class="row">
                <div id="title363" class="col-md-2">
                    <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
                </div>
                <div id="control363" class="col-md-10">
                    <div id="Control343" data-datafield="SFZ" data-type="SheetAttachment" style=""></div>
                </div>
            </div>
            <div class="row">
                <div id="title367" class="col-md-2">
                    <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
                </div>
                <div id="control367" class="col-md-10">
                    <div id="Control345" data-datafield="JSZ" data-type="SheetAttachment" style=""></div>
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
                    <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">客户银行卡面复印件</span>
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
                <table id="Control444" data-datafield="RentalAssetAccessoriesTable" data-type="SheetGridView" style="width:99%;" class="SheetGridView">
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
    <div id="example" class="modal fade in" aria-hidden="true">
        <div class="modal-header">
            <a class="close" data-dismiss="modal">×</a>
            <h3 style="text-align: center;">风险敞口</h3>
        </div>
        <div class="modal-body">
            <div style="text-align: left; padding: 0 5px; font-weight: 700; background-color: #ccc; height: 30px; line-height: 30px;">CAP：风险敞口</div>
            <div style="height: auto; min-height: 90px; max-height: 160px; overflow-y: scroll;">
                <table class="SheetGridView" style="width: 1500px;">
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
                <table class="SheetGridView" style="width: 1500px;">
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
            <div style="text-align: left; padding: 0 5px; font-weight: 700; background-color: #ccc; height: 30px; line-height: 30px;">CMS：风险敞口</div>
            <div style="height: auto; min-height: 90px; max-height: 160px; overflow: scroll;">
                <table class="SheetGridView" style="width: 2400px;">
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
                <table class="SheetGridView" style="width: 2400px;">
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
        <div class="modal-footer" style="position: absolute; right: 0; top: 450px; width: 100%; height: 50px; padding-top: 5px;">
            <span style="font-weight: 600; float: left; height: 20px; line-height: 20px;">总敞口金额：</span>
            <span id="fullckje" style="display: inline-block; float: left; min-width: 100px; max-width: 150px; height: 20px; line-height: 20px; border: 1px solid gray; text-align: center;"></span>
            <span style="font-weight: 600; float: left; height: 20px; line-height: 20px; padding-left: 10px;">信贷申请系统敞口金额：</span>
            <span id="capckje" style="display: inline-block; float: left; min-width: 100px; max-width: 150px; height: 20px; line-height: 20px; border: 1px solid gray; text-align: center;"></span>
            <span style="font-weight: 600; float: left; height: 20px; line-height: 20px; padding-left: 10px;">贷后管理系统敞口金额：</span>
            <span id="cmsckje" style="display: inline-block; float: left; min-width: 100px; max-width: 150px; height: 20px; line-height: 20px; border: 1px solid gray; text-align: center;"></span>
            <span style="font-weight: 600; float: left; height: 20px; line-height: 20px; padding-left: 10px;">共借人/担保人信贷敞口：</span>
            <span id="gjrcapck" style="display: inline-block; float: left; min-width: 100px; max-width: 150px; height: 20px; line-height: 20px; border: 1px solid gray; text-align: center;"></span>
            <span style="font-weight: 600; float: left; height: 20px; line-height: 20px; padding-left: 10px;">共借人/担保人贷后敞口：</span>
            <span id="gjrcmsck" style="display: inline-block; float: left; min-width: 100px; max-width: 150px; height: 20px; line-height: 20px; border: 1px solid gray; text-align: center;"></span>
            <a class="btn" data-dismiss="modal">关闭</a>
        </div>
    </div>
    <script type="text/javascript">
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
                if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
                    $(ctrl1).removeClass("fa-angle-double-down");
                    $(ctrl1).addClass("fa-angle-double-right");
                }
                else {
                    $(ctrl1).removeClass("fa-chevron-down");
                    $(ctrl1).addClass("fa-chevron-right");
                }
                $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
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
                $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
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
                //    alert("出错了");
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
        //敞口
        function getCkje() {
            debugger;
            var IdCardNo = $("#ctrlIdCardNo").val();
            var applicationno = $("#applicationNo").val();
            if (!IdCardNo || IdCardNo == "" || !applicationno || applicationno == "") {
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
                            capckje = capckje * 1 + result[i]['ROUND(CE.NET_FINANCED_AMT)'] * 1;
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
                        tr += "<td>Old Request</td>";
                        tr += "<td>" + result[i].APPLICATION_STATUS_DTE.split(" ")[0] + "</td>";
                        tr += "<td>" + result[i].NO_OF_TERMS + "</td>";
                        tr += "<td>" + result[i].ASSET_MAKE_DSC + "</td>";
                        tr += "<td>" + result[i].ASSET_MODEL_DSC + "</td>";
                        tr += "<td>" + result[i].ASSET_BRAND_DSC + "</td>";
                        tr += "</tr>";
                        $("#Application").append(tr);
                    }
                    $("#capckje").html(capckje);
                    console.log(result);
                    getCMSCKJE();
                },
                //error: function (msg) {
                //    alert("出错了");
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
                            tr += "<td><input onchange=\"capcheck(this,'gjrcapck')\" data-number=\"" + result[i]['ROUND(CE.NET_FINANCED_AMT)'] + "\" type=\"checkbox\"/></td>";
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
            var IdCardNo = $("#ctrlIdCardNo").val();
            var applicationno = $("#applicationNo").val();
            var fullckje = 0;
            var cmsckje = 0;
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
                    if (!$("#Application").find("td").length && !$("#cmsApplication").find("td").length) {
                        $("#control3571").find("span").css({ "color": "#000" });
                        $("#control3571").find("a").hide();
                    };
                },
                //error: function (msg) {
                //    alert("出错了");
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
                type: "POST",
                async: false,
                dataType: "json",
                success: function (result) {
                    if (result.Result == 'nofind') {
                        alert("未查询到此单");
                    }
                    else {
                        $("#aclick").attr("href", "SRetailApp.aspx?SheetCode=SRetailApp&Mode=View&InstanceId=" + result.Result + "&SheetCode=SRetailApp");
                        document.getElementById("aclick").click();
                    }
                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function capcheck(ts, ck) {
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
                url: "/Portal/RSHandler/Index", // 19.6.28 wangxg
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
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
            return false;
        }

        function rsfkClick() {
            var InstanceId = '<%=this.ActionContext.InstanceId%>';
            var rsfkurl = '<%=ConfigurationManager.AppSettings["rsfkurl"] + string.Empty%>';
            var url = "../view/rsfkresultTEST.html?&rsfkurl=" + rsfkurl + "&InstanceId=" + InstanceId;
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
            debugger;
            //共借人
            if ($("#Control28").val() == "是" || $("#Control164").val() == "是") {
                $(".bannerTitle")[3].click();
            }
            //担保人
            $(".bannerTitle")[4].click();
            //资产
            //金融
            //附件
            $(".bannerTitle")[7].click();
            //审核
            var IsWork = $.MvcSheetUI.QueryString("Mode");
            if (IsWork == "work") {
                $(".bannerTitle")[0].click();
            }
            //初始化状态
            showliuyan();
            $("div[data-isretract='true']").each(function () {
                $(this).find('a').click();
            })
            hidepboc();
            //隐藏融数
            if ($("#Control380").val() == "overtime") {
                $("#divBascSQRInfo").find(">div:eq(0)").find("a").hide();
                $("#rsmanchk").show();
                //$("#rsmanchk").on('click', rsmanchk());
                getRS();
                getNciic();
            } else {
                getRS();
                getNciic();
            }
        }
        function hidepboc() {
            //共借人不存在隐藏
            if ($("#Control124").val() == "" && $("#Control27").val() == "") {
                $("#aclickh").find("a:eq(1)").hide();
                $("#Label6").parent().hide();
                $("#divGJR").hide();
            }
            //担保人不存在隐藏
            if ($("#Control211").val() == "") {
                $("#aclickh").find("a:eq(2)").hide();
                $("#Label7").parent().hide();
            }
        }
        //人工查询
        function rsmanchk() {
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
                    $("#rsmanchk").attr("disabled", "disabled");
                    $("#searching").html("查询中...");
                    timegets();
                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
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
                    if (result.code == "00") {
                        $("#divBascSQRInfo").find(">div:eq(0)").find("a").show();
                        $("#searching").html("");
                        hidepboc();
                        printtext(result);
                        getNciic();
                    } else {
                        setTimeout("timegets()", 10000);
                    }
                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        var arrrsrq = [];
        arrrsrq['localreject'] = "东正本地规则<span style='color:red;'>拒绝</span>";
        arrrsrq['cloudaccept'] = "云端规则<span style='color:red;'>通过</span>";
        arrrsrq['cloudreject'] = "云端规则<span style='color:red;'>拒绝</span>";
        arrrsrq['cloudmanual'] = "云端规则返回<span style='color:red;'>转人工</span>";
        arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
        function getRS() {
            //var result = { "reqID": "5168553c-145d-49cf-af3c-583355ea5f1b", "data": { "action": "cloudreject", "limit": "", "scorecard": "446.71386199999995" }, "code": "00", "msg": "同盾数据：建议拒绝,", "sqr_reject_code": "RF010", "gjr_reject_code": "", "dbr_reject_code": "", "ds": { "sqr": { "6594111": { "score": 5, "conditions": [{ "hits": [{ "evidenceTime": 1511845280000, "fraudTypeDisplayName": "信用异常", "riskLevelDisplayName": "中", "value": "441322197910265235" }, { "evidenceTime": 1515480401000, "fraudTypeDisplayName": "异常借款", "riskLevelDisplayName": "中", "value": "441322197910265235" }], "type": "grey_list" }], "ruleId": "6594111" }, "6594291": { "score": 5, "conditions": [{ "hits": [{ "evidenceTime": 1511845280000, "fraudTypeDisplayName": "信用异常", "riskLevelDisplayName": "中", "value": "13428055390" }, { "evidenceTime": 1515480401000, "fraudTypeDisplayName": "异常借款", "riskLevelDisplayName": "中", "value": "13428055390" }], "type": "grey_list" }], "ruleId": "6594291" }, "6594771": { "score": 12, "conditions": [{ "dimType": "id_number", "list": ["13428055390", "134※※※※5266", "134※※※※0359", "0"], "result": 4, "subDimType": "account_mobile", "type": "frequency_distinct" }, { "dimType": "id_number", "list": ["115※※※※796@qq.com"], "result": 1, "subDimType": "account_email", "type": "frequency_distinct" }, { "dimType": "id_number", "list": ["广东省 ※※※※※※※※※※※※※※※※※路78号", "石湾镇※※※※※※※※※55号"], "result": 2, "subDimType": "home_address", "type": "frequency_distinct" }], "ruleId": "6594771" }, "6595011": { "score": 10, "conditions": [{ "dimType": "id_number", "list": ["广东省 ※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※费站附近", "东平路※※※※※10号"], "result": 2, "subDimType": "organization_address", "type": "frequency_distinct" }], "ruleId": "6595011" }, "6595071": { "score": 10, "conditions": [{ "hits": [{ "count": 1, "industryDisplayName": "小额贷款公司" }, { "count": 1, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 1, "dimType": "account_mobile", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 2, "resultsForDim": [{ "count": 2, "dimType": "account_mobile" }, { "count": 2, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595071" }, "6595081": { "score": 9, "conditions": [{ "hits": [{ "count": 1, "industryDisplayName": "一般消费分期平台" }, { "count": 1, "industryDisplayName": "融资租赁" }, { "count": 1, "industryDisplayName": "信用卡中心" }, { "count": 1, "industryDisplayName": "小额贷款公司" }, { "count": 1, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 1, "dimType": "account_mobile", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "信用卡中心" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "信用卡中心" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 5, "resultsForDim": [{ "count": 5, "dimType": "account_mobile" }, { "count": 5, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595081" }, "6595091": { "score": 30, "conditions": [{ "hits": [{ "count": 3, "industryDisplayName": "一般消费分期平台" }, { "count": 1, "industryDisplayName": "互联网金融门户" }, { "count": 1, "industryDisplayName": "第三方服务商" }, { "count": 1, "industryDisplayName": "大型消费金融公司" }, { "count": 1, "industryDisplayName": "融资租赁" }, { "count": 1, "industryDisplayName": "信用卡中心" }, { "count": 2, "industryDisplayName": "小额贷款公司" }, { "count": 10, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 3, "dimType": "account_mobile", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "互联网金融门户" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "第三方服务商" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "大型消费金融公司" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "信用卡中心" }, { "count": 2, "dimType": "account_mobile", "industryDisplayName": "小额贷款公司" }, { "count": 10, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 3, "dimType": "id_number", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "互联网金融门户" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "第三方服务商" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "大型消费金融公司" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "信用卡中心" }, { "count": 2, "dimType": "id_number", "industryDisplayName": "小额贷款公司" }, { "count": 10, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 20, "resultsForDim": [{ "count": 20, "dimType": "account_mobile" }, { "count": 20, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595091" }, "6595101": { "score": 4, "conditions": [{ "hits": [{ "count": 2, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 2, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 2, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 2, "resultsForDim": [{ "count": 2, "dimType": "account_mobile" }, { "count": 2, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595101" }, "ds6__policy_set_name": "借款事件_网站_20170720", "ds6__risk_type": "suspiciousLoan_reject", "spend_time": 47, "ds6__policy_name": "借款事件_网站_20170720", "ds6__final_score": 85, "ds6__final_decision": "Reject", "ds6__policy_set": [{ "ds6__policy_score": 85, "ds6__policy_mode": "Weighted", "ds6__risk_type": "suspiciousLoan", "ds6__policy_decision": "Reject", "ds6__policy_name": "STARK借款_网站", "ds6__policy_uuid": "1a2c7963355e4b13895576231fb9cdfc", "ds6__hit_rules": [{ "score": 5, "decision": "Accept", "name": "身份证命中中风险关注名单", "id": "6594111", "uuid": "0aa7ff6fbb684c3f80891dbb4f89b248", "parentUuid": "873cc0e156b04f6ead012b46768f3688" }, { "score": 5, "decision": "Accept", "name": "手机号命中中风险关注名单", "id": "6594291", "uuid": "6fb19669dfd54b58978007e6600964ee", "parentUuid": "63dccc42db734b1ebe1773fc635d95af" }, { "score": 12, "decision": "Accept", "name": "3个月内身份证关联多个申请信息", "id": "6594771", "uuid": "792d6b5060904ac683285fd2df39e2b2", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "3个月内申请人身份证关联工作单位地址个数大于等于2", "id": "6595011", "uuid": "aa62cadc312f4a85957d6c160c7466c0", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "7天内申请人在多个平台申请借款", "id": "6595071", "uuid": "1f157ed863c84c2481e92129ee0adc27", "parentUuid": "" }, { "score": 9, "decision": "Accept", "name": "1个月内申请人在多个平台申请借款", "id": "6595081", "uuid": "56777b55175a4b4caac02b932c35ce72", "parentUuid": "" }, { "score": 30, "decision": "Accept", "name": "3个月内申请人在多个平台申请借款", "id": "6595091", "uuid": "e1552335f5e84d10babb4929a287b3c8", "parentUuid": "" }, { "score": 4, "decision": "Accept", "name": "3个月内申请人在多个平台被放款_不包含本合作方", "id": "6595101", "uuid": "8ee79e36dd8248b59592c0aa46af2324", "parentUuid": "" }] }, { "ds6__policy_score": 0, "ds6__policy_mode": "Weighted", "ds6__risk_type": "applySuspicious", "ds6__policy_decision": "Accept", "ds6__policy_name": "欺诈行为_网站", "ds6__policy_uuid": "3d27dfa14e04440e94d171975bae2a00" }], "success": true, "seq_id": "1517549293866986F47C82BCA7833569", "ds6__hit_rules": [{ "score": 5, "decision": "Accept", "name": "身份证命中中风险关注名单", "id": "6594111", "uuid": "0aa7ff6fbb684c3f80891dbb4f89b248", "parentUuid": "873cc0e156b04f6ead012b46768f3688" }, { "score": 5, "decision": "Accept", "name": "手机号命中中风险关注名单", "id": "6594291", "uuid": "6fb19669dfd54b58978007e6600964ee", "parentUuid": "63dccc42db734b1ebe1773fc635d95af" }, { "score": 12, "decision": "Accept", "name": "3个月内身份证关联多个申请信息", "id": "6594771", "uuid": "792d6b5060904ac683285fd2df39e2b2", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "3个月内申请人身份证关联工作单位地址个数大于等于2", "id": "6595011", "uuid": "aa62cadc312f4a85957d6c160c7466c0", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "7天内申请人在多个平台申请借款", "id": "6595071", "uuid": "1f157ed863c84c2481e92129ee0adc27", "parentUuid": "" }, { "score": 9, "decision": "Accept", "name": "1个月内申请人在多个平台申请借款", "id": "6595081", "uuid": "56777b55175a4b4caac02b932c35ce72", "parentUuid": "" }, { "score": 30, "decision": "Accept", "name": "3个月内申请人在多个平台申请借款", "id": "6595091", "uuid": "e1552335f5e84d10babb4929a287b3c8", "parentUuid": "" }, { "score": 4, "decision": "Accept", "name": "3个月内申请人在多个平台被放款_不包含本合作方", "id": "6595101", "uuid": "8ee79e36dd8248b59592c0aa46af2324", "parentUuid": "" }], "ds1__product__operation": "null", "ds1__product__result": "null", "ds2__als_d7_id_bank_allnum": "null", "ds2__als_d7_id_nbank_p2p_allnum": "null", "ds2__als_d7_id_nbank_mc_allnum": "null", "ds2__als_d7_id_nbank_cf_allnum": "null", "ds2__als_d7_id_nbank_ca_allnum": "null", "ds2__als_d7_id_nbank_com_allnum": "null", "ds2__als_d7_cell_bank_allnum": "null", "ds2__als_d7_cell_nbank_p2p_allnum": "null", "ds2__als_d7_cell_nbank_mc_allnum": "null", "ds2__als_d7_cell_nbank_cf_allnum": "null", "ds2__als_d7_cell_nbank_ca_allnum": "null", "ds2__als_d7_cell_nbank_com_allnum": "null", "ds2__als_m6_id_avg_monnum": "null", "ds2__als_m6_id_tot_mons": "null", "ds2__als_m6_id_max_monnum": "null", "ds3__busiData__last3MonMuliAppCnt": "null", "ds3__busiData__last1MonMuliAppCnt": "null", "ds3__busiData__records__credooScore__0": "null", "ds4__user_gray__phone_gray_score": "null", "ds4__near1mselectcount": "null", "ds4__near3mselectcount": "null", "ds5__zm_score": "null", "ds5__score": "null", "ds6_final_score": "null", "ds7__fxtj__zhixing": "null", "ds7__fxtj__caipan": "null", "ds7__fxtj__shenpan": "null", "ds7__fxtj__weifa": "null", "ds7__fxtj__qianshui": "null", "ds7__fxtj__feizheng": "null", "ds7__fxtj__qiankuan": "null", "ds8__CDCA002__0": "null", "ds8__CDCA003__0": "null", "ds8__CDCA004__0": "null", "ds8__CDCA005__0": "null", "ds8__CDTB001__0": "null", "ds8__CDTB004__0": "null", "ds8__CDTB016__0": "null", "ds8__CDTB019__0": "null", "ds8__CDTB031__0": "null", "ds8__CDTB033__0": "null", "ds8__CDTB045__0": "null", "ds8__CDTB047__0": "null", "ds8__CDTP033__0": "null", "ds8__CDTP046__0": "null", "ds8__CDTP047__0": "null", "ds8__CDTP060__0": "null", "ds8__CDTC001__0": "null", "ds8__CDTC003__0": "null", "ds8__CDTC004__0": "null", "ds8__CDTC006__0": "null", "ds8__CDCT001__0": "null", "ds8__CDCT003__0": "null", "ds8__CSRL002__0": "null", "ds8__CSRL004__0": "null", "ds8__CSWC001__0": "null" }, "gjr": null, "dbr": null } };
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
                dataType: "json",
                success: function (result) {
                    debugger;
                    printtext(result);
                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function getNciic() {
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            var nciicurl = '<%=ConfigurationManager.AppSettings["nciicurl"] + string.Empty%>';
            //var rquet = { "reqID": "3ad7e98d-f580-4a95-99ad-749338da60db", "code": "00", "msg": "成功", "sqr": { "result_gmsfhm": "一致", "result_xm": "一致", "result_cym": "", "result_xb": "一致", "result_mz": "一致", "result_csrq": "一致", "result_whcd": "不一致（高于库中级别）", "result_hyzk": "不一致", "result_fwcs": "此项无记录", "result_csdssx": "市不一致", "result_jgssx": "省一致", "result_ssssxq": "省一致/市一致", "result_zz": "不一致", "xp": "/9j/4AAQSkZJRgABAgAAAQABAAD//gAKSFMwMQQ4CgdnBAC0AgD/2wBDAA4KCw0LCQ4NDA0QDw4RFSQXFRQUFSwfIRokNC43NjMuMjE5QFJGOT1OPjEySGFITlVYXF1cN0VlbGRZa1JaXFj/2wBDAQ8QEBUTFSoXFypYOzI7WFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFj/wAARCAB2AGADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDvdR1C3063Ms7f7qAjc3IHAJ561jWlhNrs4v8AVFK22P3FuCRwe5/x7/TFLpuky6hP/aOrje7ZCW7KRsweOPTg8c5zmujoAwdX0O1kWW7EksCrGPNSLGHRR0A7HgY7cDijwjB5OkGZtmZpCwI64HGD+IP50njC+jttJ8h8b7hgFycYAIJP8h+NcTqniW5kgS1hkKQRIIxgYLADGTQlcDvdQ8R6bYMUlnDOOCqcmsSbx9bByI7V2XsSwBP4YNedSXBYkliT9etRNKApJ/SnYR2eqeMLfUrq0jkt3W2ibdKPM4Ycew7Z5961v7b/AOEpmNpZzJbWuBv87G9+QentjoPxPNeXJ877m6fzqzHM0LAxnbjpiiwz1/Wbaz0/w/cJBDEgfaoBJyx3Z65ySOT+HpWppqlNNtFOMrCgOCCOg7jrXlll4imuza6feyE2wkDM2RkD1z7DNepLf2jQLP8AaIliY4DlwBn8aQBqdx9k024nDbGVDtOM4Y8D9cVT8NWpttHiLAhpiZSCQevT9AKp+IZF1B9PsreRJFnk3Fk+cgDjPB6ct+XtW7LJDZ2xdysUMa+mAB6Af0oAloorm9Z8RhYLiLTF8+RInZ5R92MDqff+XTr0oA4fxnqw1LxA6xH9zbjy8+pBOT+ZP5VgldxwDk+1VQ7POSTneSST3rrdFsLeC0F1dlQD03USlZDirmGNPnKGRk2oOpNM/sq4naLA2rK22Mt3PT+tdZaWsmryiWYGOzU5WPP3j9f6/gKs2lut1rc0mAIrTCIuAMHp09Mgn8qz52aciMKPwtMBhnAx7VBc6BPApIUsPau+2Y5FYWr6kNjQ2Z3nGXkXkKPY/j1/r0lTky3BI8/cvHMSAT6Vs+HtQigv0GogywNwRuIx78VspokT6eHYZlkXdkeh6CuQu42t5HQ9RxWkZKWhjKLR6PZQT2t0b7T4jdQxTSRJt+Y9OCcdsHt+nFLfT3F5f7byMz3CDCW0B+VTzkHGc9OQPXqMYqt4Vv72/wBBi0/To9rIf31xnaQdwxyPbjuSO3FdppWmRaZbmOMl2Y5dyBknH8vb3qiTJu76bXZzYaYxW2x+/uCCOD2H+efpmovEllbaZ4RureEYaQBQxUbnOd3JA7DNdDY2cNhbLBbrtQdSerH1PvVbXoILjR7lLmRo4wu4lT37cZweexoA8MTCzn0XjHrXbaXp899FG93vjt0xti6buv5fX3rC0vS9+rIsoydxYZ9B3ru2DIgSIAEDAyOlRUZrTRLuSCEtwkca54HAAqhogS20x7yZ/wDWks7sfQkfjzn86ke3uWjZSylHBUg9wayJreCzULeTyS7eYoAccZ5B9M+vHeoSNGy/PdXGrObeyDJaniSYrj6j8scdfwqK/so4FsrKJOJpAZHUfMcADPtwSaSSznmtHnvf3UKRl0t4vlAIyeRzj+fP4VSjOoOtnd20IlaNDGM9hkgfof0osI6K6dI4meVgqKMkmvOtfkWe9LxKVjPr1zXWtaX906PqJAiBz5KH+f8Ak9e1ZGpWi3cFzeQjbDGVjQdcgAD69APzogrMU3dHUfDEY0a6GOPP6+vyiu1ZlRCzsFVRkknAArmPAFq1t4dUsMGWRnH06f0q94m1BbWwa3Rh504247he5/p+PtWzMDarn/Fsjta21pFu8y4l4AOA2Ox/Ej8q6CuajljuPFdzcyy7YLOPYHPyqp+7gk+5agDLS3L6hDcFdoWHZgdBjAH6VbmV2UiIgN6kZxU88YjuCEIaNgWUg5BB5/rWdeahsf7LaqZLtjgDHC8Zz/n8axd2zpVkZmozXltdRRQzySy4y0a84/CrmmaZJHJ9pvyJbhiD8wzt/wA8fTFXNO01LWMSzfPcN8zMwyQT1AP4/jVp3DEBeewxQxpX3KmtyhNKl2sFZgFGe+TyPyzVRbW9j0+OOBzEfLBAxzk8nPpzTNSJu9Vt7Qn9yg3uBn9fwx+dbgIdQQef5UugW1MK6F3b6fIbiXzFMZUjHIJ449uaW2tC/h8xFQGkQt9cng/lirWvbxpj4AYFhvPoM/44qrJqk1yxj06BnOBl2H3cj8vzoVxOxtQ61HZ6Vb2trHvugoREC5HBA5wep6jFV9HtJbvxA8l2xkeA75G5xvHRc9sH8Pl44q1p9pFoGim8lO+9dD8zHqWOQMZ5Hf161e8NWZtdLV3A3znzOgzjsM/r+NbI53uO1jWV0/EUCCe4IJKBvuADOWA59/pnmszQtDt7y1F7es05mz8pJGDuIJJByTxV+00CKG0l81vMvJkdXnOW2lhg4H9ep59cVkajp1/p+lSRzXsZtE+WOMDBfLg9Mde/U4we2aYjLS6uW04WdojtOXY7s8ImBwPTnP5+prQ02wisASmWkYYZz1+g9BW3p2n+RoKxtEqzum5sLgk5JAPfI6VmZrOehtTJpQzkKDgd6pzLciUMskccCDJ3Ln9c8VNLKYkLYJz2AzWPrN/IYI7eJZN8zYIAwCPT9RUrU1vYh0yO8uGlvlkQSs+0grnjg4HP0/KugiVxh2wCeoFU4ZktYEhjgl8tR1IqaO73PtAbB9VI/nSdgs0WnYbeOtVDdW1oyLMcLwBGmM49h6VU1HUCj/ZrUF7ljjGPu8ZzV7wzpIW9a5uQks3LZxwpz1Hv70RWpEnoLc30uvTW2niAWoLbyzEscbSQeg7E/Xiuurn9If7X4i1C7Qr5ar5fDZz0AI9vl/WtTU9Rh0238yX5mPCIDyx/w963OcuVz+sst5rmnWCsG2P5kqMcqR1wR64B6+vvW+zKiMzsFVRkknAArA8Oo1xe32ouj7ZnxC7nnbk5H4YUfhigDoK5nVYxbXrBRhWG4D69f1ro5pY4ImklcIijJY9BXI3Wox6vJK8S7UjbYpPUgc59utTPYuD1Jo2BXJ5FYNzLENdlllmKrEmE2DPOOn6mmXGqTRSNbWwLy92HIXmpNImt4Lc3c0imVyTufHy8kdfeslobXuaVpi7jMkc8jorFeRiqt1emdhaaed8jfel7KPY/1/Kse3vJrhrgI6xpcPmVx26nA/OteCSG2h8uAfViBk/U0nZDu2PtLGKzJCZeRuCx/p6CushEWk6a005Pyjc+Bkk9hXL2jD7QjuSQpBP0BrW1m/XUobazsHEhuiGJHQLnjPHrz7baun3MqnRFDRZL63hnjs7OR5psYlbhVwMjqMZwc8nuPx29P0YJOt7fO0923zENjCHA/Mj8varVzd22j2USysxVVCIowWbAxWVPrOppafbBawRWzEBBIxLHPccjPT06H8a1Mi14ou0ttIdC5V5iEXBxxnJ/DHH41T/t3T9FsIbZXE8kaAMIjkbu5yffNeeXuoXN/qKNczSS+WM4ZuB+H5UxpCQT3PAq1EVza1rxDcapIQSY4AfljB/U+pqnpl1PKj2lqSJTJlnHQLisZ5WlJjiIxjk1qeHpVtL4Jn5ZRtJPr2/w/GpqL3dC4bnQw6fHbw7EGWP3mPUmsjUdLhgieccE8BCeMn0/nXTgAjp1rK1NVuNQtbUfMA26RQeMf44B/OuVbnS0rFOxsZYrVAyqpIyeOefWtCO2IAGDk+1aHlrjkZo2gcnjHSpKK5KwRsxIwASTXGaXrc+nXYu4iCyAhEYZHTv+f61veJb5bXT5Ig+JZflUDrjv+ma5SCPyxk/e/lW9FOxz1bXsd/oWu2V9fm71eUR3WcRK3+rT6H+X59a2/EMiXaWFtG6lbiYYkU5A7fj979K8pDYOD0NPt7qZLtWjmkXyuVKsRz/n+VbWMRkYw8rdyxFNeRp2MafKo6570UVQEyqI02qOKNxzkHBzwR60UU3sNHQ6L4h+1kQTxHzBxvXGD+FXtKAk1C/kbJcPtBJ7ZPH6CiiuSZ0LoauzJ61h6/rw0v8AcxxF5yOC33R/WiioitS5PQ4dppbu7V55Gdupyc1bHSiiuyOxysQnAz6VBGWclAcbjyaKKbJP/9k=" }, "gjr": { "result_gmsfhm": "一致", "result_xm": "一致", "result_cym": "", "result_xb": "一致", "result_mz": "一致", "result_csrq": "一致", "result_whcd": "不一致（高于库中级别）", "result_hyzk": "一致", "result_fwcs": "不一致", "result_csdssx": "市一致", "result_jgssx": "省一致", "result_ssssxq": "县（区）不一致", "result_zz": "不一致", "xp": "/9j/4AAQSkZJRgABAgAAAQABAAD//gAKSFMwMQBqAACRBwCY7AD/2wBDABgQEhUSDxgVExUbGRgcIzsmIyAgI0g0Nys7VktaWVRLU1FfaohzX2SAZlFTdqF4gIyRmJqYXHKns6WUsYiVmJL/2wBDARkbGyMfI0UmJkWSYVNhkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpL/wAARCADcALIDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDqWAYYYAj0NM8sLnMjhepBb+vWkLuDgsueyqpJ/nTSJGx5y5Xg7U/r/wDWoAeMldsICr/exx+A705EVM7RyepJyTSqwYZU5pGdFOGYA+negB1MZ/mCpgtnn/Z+tNBkkI4KJ+p/wpzOkQC9+yjkmgBFhC/Nk+Z3b1/+tS5kHVQfoaCZCPlRR6bm/wAKY8xj4ZMnsEOfzoAcd+MswQf7PJpqx7icgqh492+p61QvtUht8FWWWXsF5C1iXOpXc5+abaM52jpQB2AAAwOAKK4n7bcD7spH+4cUo1O+yQJ3wOoY5/nQOx2LfvDsAOB1f0+nvQsbKSVcHPdl5rlF1e93KXlDqv8ACyDB/ACrcOuKcCa3VTn70R2kfgaBHQ7XYfM+P9wYpfljUngDqTVO31GGZD5cgc88Hhvy/rVpVD4ZmDc5GOg/xoAMu/3fkX1I5NL5Y7sxPruI/lTiQoySAPU03eXH7v8A767f/XoAa+Y8bWJJ6Kec0g3Fg0qkY6ADIH5Uu4KTtzI54PP8/SnDzMHJUfhmgA35OFRj7kYH60eXuOZDkf3R0/8Ar0fOP7rewGKDKq9cg+mOtAD6Kbl/7q/i3/1qKABUVM7VC59BikLEnCDJHUnoP8ab80ucgrH0xjlv8BUoAAwBgCgCJoFk/wBaS+OnbFKsIjH7o7fY8g09nVep5PQdzUTZZsSYx1EYGSfrQAnmuy8dMkb1UkflQZYoYyxLcfeJUk/jTiz4y5CA9hyfzrm9Wv2lkMayEhfyoAs3uuEFkhXGO+cVizajPK3Mh56jsarSyOeufpSLg8ty1ABJMzNuJJ/rUe8jq2PpUhj3HgYoFqx9T+FACIRj1qUNTDaMPut+lN2Oh6EigZOGDUhbHUZNRBz34FPXkccUAPSTy2DqxDDuK27DWFG1bnLA8eYCQfx9awAMdKcGxyKAO7QRlRICGHUMTn+dKSznC/KmPvdz9K5jSNRMDKknzQ9h/c966dZoyM7wPrxQIeqhVCqMAUtMMq4yDn6cj86Z88p7KoPbvx+v+etACyS/NsU4PrjP5ChVYA/KDu6lm5P6U+ONY1wo/H1p1AERTJyYo8/X/wCtRUtFAEYjZP8AVt8v91uR+fal2yHGXCj/AGRz+tAhQfdG3/dJFR7nYcFjHyN4HP6f5/nQA4H5mWLlgfmZuR/n2p6oEyR1PUnqaVNu0bMbe2OlLQBUv5fIs5pd2CF4J9a46TKqW7mtvxJeDctujA7fmb69q53JkbmgYiIXbn86vQWmetOtrcD+GryRAdOD6ipbLSI0tFHYfWpREq445PQU5N2OBkD+Id6lTao4DE/Q5NRcuxD5Gf4QPrzTXiwNuwH6Vawzf7I/WjaqHA6nsOpouFjKls1fc3VvT0qlJCYm6cV0Bi8z7wUe2MmqdzZ8HqfrzVJktGQfQc00D161PMhXkr9cVWZse1WjNkqtjntXQaHqYP8Aosx+X+An+Vc0D7fnU0DGOQMh2sOQy9qAO8UFxlxgdl/xp9U9PuzcWqSMMnoxX1+lWRKhHDfh3/KgQ+kZgo569gOppjOdpIAAHdv8KRVZuWJUH8z9fSgB+X/ur+Lf/Woo8tP7i/lRQAhQv/rOn90dP/r05mVRliAPc0zcz/d+Vc/e7n6UqoiZYDB7sTk/nQA0mPOQxjP5foajmaONCSTIcbsE8EevpUpZn4j4H98/09ap6lH5Omy+U2045zzmgDkLyUzTMzEZY5OOgp9lFuk57VWZsyE4rQsMkHoB6mk9hrcvKAgyaeAGxu4H93uaagycjP8AvHvViJAPr3NZmyHLgfwNj1qVcN0PSlUUbBIfl4H98fypANOScL17k9KFQLnuT1JpwKxAI2B6Gl3p2YH6c0DExTJduMDk+gqTDP0+Ue45pCuPlQZ55J/zzQIyLy0ZCZAN2eoWs6aHI3LyK6V4VK/N8x9xWdc2EYYuoCk/lVpiauYOTT1fmrF3aEAuOMdVPWqe4g9qsyasdL4auGZ3gLkAjeMH/P8AkV0DEhsIxLDHBHH41xWlTGK/iYN/FhsV3CKFUBRgCgRGAync6lj6jnH4U7zF/wBr/vk0+kdggyfy9aAE3H/nm36f40UZf+6v4t/9aigCMMg4Fxx/vA04JGwzw/uTmpKikVFORuDseNnU0AS1l69OF02QLkgkDcOlXvLkK/OyucdCMCs/xEH/ALNJZh94cAUAcYW+fpWtpa5QFzyOgrM8slh7mt+1j2QhaTHEmAqQOAcdT6DmovLBOAoX3qSOEt/qzj37VJrcnVGPLDjsg7/WrIQsPmPH90dKrKskX3G57nualjlYjDCgLkoRAMBQB9Ka0Srkg7e59KC4UZP/AOumYJbc4+g9KAEHzEbxhT0H97/61SFGxhVpjPkYPI96hZQPmbP0xmgB0j7c/LuPt/jVdkL/AHj/APW/CrIjLqCzAY6KOgqCQMp55FFguVpIVKH19awLqPZKRjHpXTEZrG1ePaQ1NEyKVu21hiu9ileWFGCH5gDkEfpXn0f3ua7/AE+QSWULjjKCqMyQRlh8w/76Of06UCBF5GQRzuzUjMFGSabtL/6zp/doAjy55ErY7fu6KnooAhaYsSsQJ/2sUqxHq5O72P8AX/IqRVCjA78k+ppaAGeVH/cUfQVQ1qBnsJAGyo5wf8a0qp30m+2ZQPkbjIPX6UAjiww85K205UfMx9lGBVG4hX7dGqJsXOK1m+WpbLSI9hzySPYHP86GvY7chXdSP1FQzM8mQnyj+9TVtl8iRADlx97rSLLKajFIcIQfrU/mv3TaPXrWVa25WXM6rtH92MA/pWhG4jiP3/8AZDUNAizAVPIO4+vWnO/GVGR654qpHhjlwD+FSsVxnGT781IxGmYDIUfXNVJL+GH7xOfQ9asyjzIuBn1qpdw+eyiMGJsbSEPUehpoTJYtSikHGR2yRxU4lElVEt3WJkcM5Y5dmwSaSJNny/MQOx6D602CLTsMcHNZuqRnywSa1FTC88n+VVtTTNt9DQhM5ojGK67w7cM9gEDbyhwF9vr2rBghRid+Cv8AtCtrw4jRNPHGQU4IJq7mdjcjHVicseue3tT6YYwTkk7sYyDik8tgMLIx+vegRJRTAgIGdwPpuNFAD6a7hBz17AdTUZcY+eXb7AbSfz5/KlRCfm5QHqM8n6n+lADTud+gYdNueB9femXkbNDkyHIPYACrQAAwOBUNyw8tlHLY6UnsNbmBLEftat1A9alP7w88KP4e/wCNOc/vkydxDcKO1KylmBJxj0qehq9xwTPapFjA6UyMSAHDhj7ipBgj94Tx2bGP/r1IxhTecL/312FRykY2x5c9zUxBcEY2p0x0Jpk3y8igCFdwPUflT3YqPmHHqDUatz70rBmIYgexP+FICSM5O3dtXv61KFjU4UdPQZpsK7xyx/SpPnXGMMPToaYCeWzdflH15oMYAxjipFbccA8+nekdgV+XoO54FAEBUDpkfQ1HMiNGQx5PrT23n7oXHqaRx8nzA59etUhMrC2QoCwXGORWlpsIR/MA7bT+lUYHBRkyTg9q1bUrFH+8OGY7uhprcUti5TCxY7UPfBOOlL8zHn5R6d6XAUcYAFUZCCMY6t/30aKPMT++v50UAJ5YJy/zHpz0o2Y+6zKPQY/rSs6qcE8+gGTTGmIO0L8390nn9M0AO2Z+87H8cfyqOQFoykQCqR97HH4UbtzMJQVUdARwfqamZlX7zAfU0AYZiC9Dginv1ouXXz2CkEZ9abI+7Gzk+vaoN2ODhRz+Q6mpUTfkvz6AHpUEa4PPJ9TUqSKOrD86kCQoVXAbP+8M1UnOSd74Veu0dats+8YTn37CqZjDB4nGQ/UjvQAtt5TYbKuo6Fanl8vHJA9yaqwWf2cfuyWzS3NmbmPDMVwc5oAmQGJgQeDVgsAu4/8A66qxp5caxKS4TuegqxHkdFJJ7nigBwj34Lj8PSkYKuMKBj2pWdgOV/LmonfPCYJ/QVQDXYcdyegFMmDsgAwSex6VIBjuSfU0yTNAuoyG2AfAAJJ61uqoUYUAfSsyxx5mW6LzmtH5nPOVX07mqRE9xDgn90cE9WHSgRZbLuWNO3oOMjj07UoZW+6QfoaZAmwerf8AfRooLoDgsv50UAIIkGcbhnk/MacAqLgYCik3rtDbhg9Pek2lx+8Ax/d/x9aADLN9zhf7x/pSLCijA3f99GpKKAKs1jDK25t2fqTVK8iWGTCscHovWtTfuOEGf9rt/wDXqlfBFxhtzZ57n9KTKTKAEjHkgD0BqVRj7xyPQcCmb1zycfUYqUkDqQPrWZqK23HAwfUUzJXryKhllweuwfTmmieMDPX680AW4244VjQxZuM4P90H+ZqkJ85xhc/gakWYp90gj3qgsWVUADPOOnpUgfFU2usDkKv40zz1ZxnLeg21IF1nJHGAP7xqPqcoMe570qDdyxzjoOwpxoAjy46gfgKnjg82ME5jcnhiajA5rQUfuljAzxznoKtESdiO3gFuSSxkc9sYqxsLcuc/7I6UKrJ0O4f7XX86GZh12r+tUZ7jiQo9B2qI5lY7RtA7kdeaVULjMnPsalHAwKAIxCuP/rCipKKAIXjAbeXxJ2IH9KXzSBzG59Co6/4U9UVeQBnue5p1ADPNBbCqzEdeMY/OkcZBaVsL/dB4/OlkwcDB3HpjqPx7UxQwfdIC2OhHOPwoAeAWA6ouPu9DUdzGGgIAAA5wKk8xf9rj/ZNNLuW24Ck9s5P+AoAyHYDjqT2oQHHy5z7dBUt1bCJ923ryKjRqhmyZHIkh+9yfVagSHByACR69auPyKbhe/Xt61IxqgfxR80ARgcoKd5TsDliB+tKLdQc45p3Hcr7cn92m33qZIgFJbn1Jp5QE4UYI7+lCghsnDH+VK4hwZQvXj1pcjGcjHrSMWA4A+pNRkAnJPNCAsWqmaUYyFHJNaagKoAGAKzrSZUk8ouN78j5ck4q7iRuvA9Acf41ojF7jy2DgDJ9B2+tCrjljlv5fSkB2gAoQP9nkU7euAdwwfemIWim7175H1BFKXULuLDHrQAtFR+b/ALD/AJUUASUwvu4iweeT2FMZJSPmYMO69M/jTjKqcFWU9AMdfpQA5EC+7HqfWnVCZGb7gIz0+XP/ANalMcjfeYEZztxwfxoAduLcJ/312/ClVQo469yeppFcAAEbD6H+lPoAinjWSIh+B1z6ViFtrEVa168+z24jU/NJ1+lUHO89wPpyallRJ95PCjJp0YwxJ5J71TBdR6CniVQOpH41BoaSkEUmd/ThfX1qms3I3t8v6/jUwmyMjp7mgZIcKMDgU3dioJJ8dR+RzUDzk4x36UAWXmFRB2lPy/d9aiVS33zgelTrkDhePfigRS1CVoZIGQ4IJ+Y/hXS2VwLq0SUEAkc+xrltWJ8tSQOD61JomoC3k2SkmM9B6N61otjKW51WWfG3hf71AjA5BYHuc9aAzMAQFIPQhv8A61AkByApyOMUxC4bHLgD1xzTAgY7l477j1NO2Fvv/kOlOJA6mgBnkqO/6D/Cil3ydo//AB4UUAG/ccRgN2LZ4FKqBST1Y9SetIrFh+7GF9SP5ClxJ/eX/vn/AOvQA6kLBRknFU7u/S1X96VB7YOa5++1uWYlYiVVvl/2jQBv3WoW8IKyyAH+6uGas2fW0VSsKHgY3M5/lXPNMe55phcnrTAt6hevdTb2ABAxitS3OY1Yqc465rn85resmHkLn0qZFRJWRm7Y+tQ+WN3Ay3Qk9qt7SfYelN2DGF6VkaESwL+PrTvsv91iKmQf5NS4oAqfZ8feOfoKaVCEbRyatHkkL26mmhAvB6n9aAI0Q9XBJ9RUhyOSQo/WnAPnsB6UpUn7x/AUAZWo4MDD36ZrKjbHetnUgDxjpWHnmtYmcjc0jVPs7COUAx5/L6V1CMrqGUgg9CK89R8GtKz1ae2UIJPk/utyKok7BnAOByf5fWmjkhsbj2J4FZtlq1tKQsv7tz68qfxrVUhgCCCD0IpAJl/7q/n/APWoo3p/eX86KAIp7uC3H72VVP8AdzzWJfa95iGO2DIT/FWA0pPfAqMtTAnuLt5W3O5Y+5qDPrUYOWz2pxNAATTc0maT60CJI+WH1rc07hMEHjv1rDh/1i/Wt23wsm0857VMi4l4NkZXp6npQUHfJ+tG0Ebm4J6Y7UuSBypI9R/hWRoKqgfdJH0qTj0qMOM4AJPpinEE8swUe3+NAwLE8KMn9BTcAEknLU4DIAA2r+tOVQOlACFlHXI+opGbC7u1PPFVpiNucc9eB0/xpiKN6xMcjDpg81hd6274sLOQnPT2rDPWtImUhwNPB21Fmn7qsklWQ9DmrdrqU9uBslIX+72/Ks4txS570hnSjxIwH/Hun/fdFc3uooAQmo5DQaY55oESDgUGheaQ0AJRQe9FAEkH+tX6iugiXDArn/ernovvqPeumTtUyLiWkAAz39afikj6U4nCk+grM0GOBjB5PYU3YQRuy2OlSoONx6kU6kBECPf8jTyTxxjP50rcKT6ClQZAbuRTAjCcfN37VDPyatP0qrL1oEZuqnFmR6kCsRq2ta/49V/3v6Vi961iZy3E706m06mSGaTOKSg0ALiikooA/9k=" } };
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx",
				url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
                data:
                {
                    CommandName: "getRSResult",
                    param: "{\"reqID\": \"" + instanceid + "\" }",
                    address: nciicurl
                },
                type: "post",
                async: true,
                dataType: "json",
                success: function (result) {
                    //var result = rquet;
                    if (result.code == "00") {
                        if (result.sqr) {
                            if (result.sqr.result_xm == "不一致" || result.sqr.result_gmsfhm == "不一致") {
                                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                                $("#control21").append(span);
                            } else {
                                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                                $("#control21").append(span);
                            }
                            if (result.sqr.result_hyzk == "不一致") {
                                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                                $("#control24").append(span);
                            } else {
                                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                                $("#control24").append(span);
                            }
                        }
                        if (result.gjr) {
                            if (result.gjr.result_xm == "不一致" || result.gjr.result_gmsfhm == "不一致") {
                                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                                $("#control123").append(span);
                            } else {
                                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                                $("#control123").append(span);
                            }
                            if (result.gjr.result_hyzk == "不一致") {
                                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                                $("#control126").append(span);
                            } else {
                                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                                $("#control126").append(span);
                            }
                        }
                        if (result.dbr) {
                            if (result.dbr.result_xm == "不一致" || result.dbr.result_gmsfhm == "不一致") {
                                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                                $("#control217").append(span);
                            } else {
                                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                                $("#control217").append(span);
                            }
                            if (result.dbr.result_hyzk == "不一致") {
                                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                                $("#control220").append(span);
                            } else {
                                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                                $("#control220").append(span);
                            }
                        }
                    }
                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function printtext(results) {
            var result = results;
            if (result.code == "00") {
                $("#divBascSQRInfo").find(">div:eq(0)").find("a").show();
                $("#searching").html("");
                if (result.data) {
                    $("#fkpgjg").html(arrrsrq[result.data.action]);
                    $("#grxypf").html(Math.round(result.data.scorecard));
                    var span;
                    if (Math.round(result.data.scorecard) < 400) {
                        span = "<span style=\"color:red;padding-left:15px;\">建议拒绝</span>";
                    } else if (Math.round(result.data.scorecard) >= 470) {
                        span = "<span style=\"color:red;padding-left:15px;\">建议通过</span>";
                    } else {
                        span = "<span style=\"color:red;padding-left:15px;\">建议人工审批</span>";
                    }
                    $("#grxypf").after(span);
                }
                if (result.ds.sqr && result.ds.sqr.ds1__product__result == "不一致") {
                    var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                    $("#control105").append(span);
                } else {
                    var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                    $("#control105").append(span);
                }
                if (result.ds.gjr && result.ds.gjr.ds1__product__result == "不一致") {
                    var span = "<label><span style=\"color:red;padding-left!important:15px;\">不一致</span></label>";
                    $("#control209").append(span);
                } else {
                    var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                    $("#control209").append(span);
                }
                if (result.ds.dbr && result.ds.dbr.ds1__product__result == "不一致") {
                    var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                    $("#control301").append(span);
                } else {
                    var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                    $("#control301").append(span);
                }
            }
            if (result.sqr_reject_code.indexOf("RB021") != -1 || result.sqr_reject_code.indexOf("RB022") != -1 || result.sqr_reject_code.indexOf("RB024") != -1 || result.sqr_reject_code.indexOf("RB025") != -1 || result.sqr_reject_code.indexOf("RB009") != -1 || result.sqr_reject_code.indexOf("RB010") != -1) {
                $("#sqrhmd").html("<span style='color:red;'>是</span>");
            } else {
                $("#sqrhmd").html("<span>否</span>");
            }
            if (result.sqr_reject_code.indexOf("RB004") != -1) {
                $("#sqrwbxy").html("<span style='color:red;'>是</span>");
            } else {
                $("#sqrwbxy").html("<span>否</span>");
            }
            if (result.gjr_reject_code.indexOf("RB021") != -1 || result.gjr_reject_code.indexOf("RB022") != -1 || result.gjr_reject_code.indexOf("RB024") != -1 || result.gjr_reject_code.indexOf("RB025") != -1 || result.gjr_reject_code.indexOf("RB009") != -1 || result.gjr_reject_code.indexOf("RB010") != -1) {
                $("#gjrhmd").html("<span style='color:red;'>是</span>");
            } else {
                $("#gjrhmd").html("<span>否</span>");
            }
            if (result.gjr_reject_code.indexOf("RB004") != -1) {
                $("#gjrwbxy").html("<span style='color:red;'>是</span>");
            } else {
                $("#gjrwbxy").html("<span>否</span>");
            }
            if (result.dbr_reject_code.indexOf("RB021") != -1 || result.dbr_reject_code.indexOf("RB022") != -1 || result.dbr_reject_code.indexOf("RB024") != -1 || result.dbr_reject_code.indexOf("RB025") != -1 || result.dbr_reject_code.indexOf("RB009") != -1 || result.dbr_reject_code.indexOf("RB010") != -1) {
                $("#dbrhmd").html("<span style='color:red;'>是</span>");
            } else {
                $("#dbrhmd").html("<span>否</span>");
            }
            if (result.dbr_reject_code.indexOf("RB004") != -1) {
                $("#dbrwbxy").html("<span style='color:red;'>是</span>");
            } else {
                $("#dbrwbxy").html("<span>否</span>");
            }
        }
        //审批意见
        function hidexscsyj() {
            $("#xscsyj").find(".widget-comments").removeClass('bordered');
            $("#Control362").find(".widget-comments").removeClass('bordered');
            var Activity = '<%=this.ActionContext.ActivityCode%>';
            var IsWork = $.MvcSheetUI.QueryString("Mode");
            var len = $("#xscsyj").find(".widget-comments").find(".comment").length;
            if (len > 1 && Activity == "Activity13") {
                var num = len - 1;
                $("#xscsyj").find(".widget-comments").find(".comment:eq(" + num + ")").prevAll().hide();
                $("#xscsyj").find(".widget-comments").find(".comment:last-child").after("<a onclick=\"isshowcsyj(this)\" data-isshow=\'false\' style=\'float:right;margin-right:30px;margin-bottom:5px;\'>展开更多(" + num + ") &or;</a>");
            }
            if (Activity == "Activity14" && IsWork == "work" && $("#Control362").find(".widget-comments").length == 0) {
                $("#xszsyj").hide();
            }
            //if (Activity == "Activity13" && IsWork == "work") {
            //    $("#Control362").find("select").parent().hide();
            //    $("#Control362").next("label").hide();
            //}
        }
        //是否显示初审意见
        function isshowcsyj(ts) {
            if (ts.dataset.isshow == 'false') {
                $("#xscsyj").find(".widget-comments").find(".comment").show();
                ts.dataset.isshow = 'true';
                $(ts).html("收起 &and;");
            } else {
                var num = $("#xscsyj").find(".widget-comments").find(".comment").length * 1 - 1;
                $("#xscsyj").find(".widget-comments").find(".comment:eq(" + num + ")").prevAll().hide();
                ts.dataset.isshow = 'false';
                $(ts).html("展开更多(" + num + ") &or;");
            }
        }
        //银行卡数字验证
        function textnum(ts) {
            if (!/^[0-9]*$/.test($(ts).val())) {
                $(ts).val("");
                alert("请输入数字!");
            }
        }
        //审核信息顶部固定
        function setfixtop() {
            $("#fktop").css({ "position": "fixed", "top": $("#main-navbar").outerHeight(), "left": $("#main-navbar").offset().left, "width": $("#content-wrapper").outerWidth() });
            $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
        }
        $(window).resize(function () {
            $("#fktop").css({ "width": $("#content-wrapper").outerWidth(), "top": $("#main-navbar").outerHeight(), "left": $("#main-navbar").offset().left });
        });
        //标红
        function isred() {
            if ($("#divOriginateOUName").find('label').html() == "外网经销商") {
                $("#divOriginateOUName").find('label').css({ "color": "red" });
            }
            if ($("#control20").find("label").find("span").html() == "是") {
                $("#control20").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control167").find("label").find("span").html() == "是") {
                $("#control167").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control63").find("label").find("span").html() == "是") {
                $("#control63").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control27").find("label").find("div").html() == "非本地户口") {
                $("#control27").find("label").find("div").css({ "color": "red" });
            }
            if ($("#control165").find("label").find("span").html() == "是") {
                $("#control165").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control129").find("label").find("div").html() == "非本地户口") {
                $("#control129").find("label").find("div").css({ "color": "red" });
            }
            if ($("#control258").find("label").find("span").html() == "是") {
                $("#control258").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control223").find("label").find("div").html() == "非本地户口") {
                $("#control223").find("label").find("div").css({ "color": "red" });
            }
            if ($("#Control315").val() == "否") {
                $("#Control315").parent().find("label").css({ "color": "red" });
            }
            if ($("#control340").find("label").find("span").html() * 1 >= $("#control309").find("label").find("span").html() * 1) {
                $("#control223").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control342").find("label").find("span").html() * 1 >= $("#control309").find("label").find("span").html() * 1) {
                $("#control342").find("label").find("span").css({ "color": "red" });
            }
        }
        //初审状态下拉框
        function bindradio() {
            $("#ctl271484").find("input[type='radio']").click(function () {
                if ($(this).val() == "拒绝" || $(this).val() == "有条件核准" || $(this).val() == "取消") {
                    $("#csjjxz").show();
                } else {
                    $("#csjjxz").hide();
                }
            })
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
            isred();
            hidemsg();
            setfixtop();
            hidexscsyj();
            //---------------开始 PBOC权限控制------------------
            var acls = getAcls($.MvcSheetUI.SheetInfo.UserCode);
            //PBOC
            if ($.inArray("001", acls) == -1) {
                $("#aclickh a").remove();
            }
            //人工查询
            if ($.inArray("003", acls) == -1) {
                $("#rsmanchk").parent().empty();
            }
            //---------------结束 PBOC权限控制------------------
            //bindradio();
            //共借人时候机构贷
            if ($("#Control27").val() && $("#Control27").val() != "") {
                $("#main-navbar").find("h3").html("个人汽车贷款申请<span style=\"color:red;\">(公牌)</span>");
                $("#Label6").html("共借人信息（公司）<span style=\"color:red;font-weight:200;font-size:14px;\">公司信息请去cap查看</span>");
            }
            GetGuessCustomer();//获取客户测算指标信息
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
        // 表单验证接口
        $.MvcSheet.Validate = function () {
            // this.Action 表示当前操作的按钮名称
            var nameText = $.MvcSheetUI.GetControlValue("mvcName");    // 根据数据项编码获取页面控件的值
            var applicationNumber = $("#Control11").val();
            var AccountName = $("#Control415").val();
            var AccountNumber = $("#Control416").val();
            var BankName = $("#ctl64724").val();
            var BranchName = $("#ctl875733").val();
            var EngineNo = $("#Control297").val();
            var VinNo = $("#Control298").val();
            var v = $.MvcSheetUI.MvcRuntime.executeService('CAPService', 'UpdateDDBank', { applicationNumber: applicationNumber, AccountName: AccountName, AccountNumber: AccountNumber, BankName: BankName, BranchName: BranchName, EngineNo: EngineNo, VinNo: VinNo });
            // 填写申请单环节，设置 mvcName 必填
            if (v && v == "Success") {
                //$.MvcSheetUI.GetElement("mvcName").focus();
                alert("请填写名称.");
                return false;
            }

                //信审初审
            if (this.Action == "Submit" && $.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
                if (!$.MvcSheetUI.GetElement("xsjjlyzx").is(":hidden")) {
                    var ope = $.MvcSheetUI.GetControlValue("csshzt");
                    if (ope != "核准") {
                        var category1 = $.MvcSheetUI.GetControlValue("xsjjlyzx");
                        var category2 = $.MvcSheetUI.GetControlValue("xsjjlyzx1");
                        var category2OptionNum = $.MvcSheetUI.GetElement("xsjjlyzx1").find("option").length;
                        if (!category1 || category1 == "") {
                            alert("请选择" + ope + "的原因");
                            return false;
                        }
                        else if (category2OptionNum > 1 && (!category2 || category2 == "")) {
                            alert("请选择" + ope + "的原因");
                            return false;
                        }
                    }
                }
            }
            return true;
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
        //客户指标测算
        function GetGuessCustomer() {
            var appno = $.MvcSheetUI.GetControlValue("applicationNo");
            var certno = $("#ctrlIdCardNo").val();          //获申请人取证件编码
            var gjrcertno = $("#Control124").val();         //获共借人取证件编码
            var isgjr = $("#Control28").val();              //是否隐藏共借人
            var loanAmt = $("#ctrlfinancedamount").val();   //放款金额
            var periods = $("#Control321").val();           //期数
            var dzcarloan = "";                             //东正车贷月应还款额
            if (gjrcertno != "") { dzcarloan = parseFloat(loanAmt) / parseInt(periods); }
            else { dzcarloan = 0.7 * parseFloat(loanAmt) / parseInt(periods); }

            //alert("证件编码:" + certno + ",有无共借人:" + isgjr + "," + "放款金额:" + loanAmt);
            //alert("共建人证件编码:" + gjrcertno + "东正车贷月应还款额:" + dzcdRepayAmtOfMonth);
            $("#ControIncomOfMonth_ctl").attr("disabled", "disabled");
            $("#ControDabtsOfMonth_ctl").attr("disabled", "disabled");
            $("#ControAssetValuation_ctl").attr("disabled", "disabled");
            $("#ControRepayLoan_ctl").attr("disabled", "disabled");
            $("#ControgIncomOfMonth_ctl").attr("disabled", "disabled");
            $("#ControgDabtsOfMonth_ctl").attr("disabled", "disabled");
            $("#ControgAssetValuation_ctl").attr("disabled", "disabled");
            $("#ControgRepayLoan_ctl").attr("disabled", "disabled");
            //获取申请人指标信息
            $.ajax({
                url: "/Portal/PCMHandler/Index",//wangxg 19.7
                //url: "../../../ajax/PCMHandler.ashx",
                data: { CommandName: "GetConnect", certno: certno, dzcarloan: dzcarloan,appno:appno },
                type: "POST",
                async: true,
                dataType: "text",
                success: function (result) {
                    result = JSON.parse(result.replace(/\n/g, ""));
                    if (result.Result == "Success") {
                        $("#ControIncomOfMonth_ctl").val(parseFloat(result.C_IncomOfMonth));//月收入估值 
                        $("#ControDabtsOfMonth_ctl").val(parseFloat(result.C_DabtsOfMonth)); //月应还债务
                        $("#ControAssetValuation_ctl").val(parseFloat(result.C_AssetValuation));//客户资产估值
                        $("#ControRepayLoan_ctl").val(parseFloat(result.C_RepayLoan)); //客户月还贷能力
                        //alert("月收入估值：" + C_IncomOfMonth + "月应还债务:" + C_DabtsOfMonth + "客户资产估值:" + C_AssetValuation + "客户月还贷能力" + C_RepayLoan);
                            debugger
                      	    var hked = parseFloat(loanAmt) / parseInt(periods);
                            var cz = (parseFloat(result.C_RepayLoan) - hked).toFixed(2);
                            $("#srfzc").html(cz);
                            if (cz < 0) {
                                $("#srfzc").css("color","red");
                            }
                    }

                },
                //error: function (msg) {
                //    alert("出错了");
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
            //获取共借人指标
            if (gjrcertno == "") { $("#divgLine").hide(); }
            else {
                if (isgjr == "否") { $("#divgGGustom").show(); $("#divgLine").hide(); }
                $.ajax({
                    url: "/Portal/PCMHandler/Index",//wangxg 19.7
                    //url: "../../../ajax/PCMHandler.ashx",
                    data: { CommandName: "GetConnect", certno: gjrcertno, dzcarloan: dzcarloan },
                    type: "POST",
                    async: true,
                    dataType: "text",
                    success: function (result) {
                        result = JSON.parse(result.replace(/\n/g, ""));
                        if (result.Result == "Success") {
                            $("#ControgIncomOfMonth_ctl").val(parseFloat(result.C_IncomOfMonth));//月收入估值 
                            $("#ControgDabtsOfMonth_ctl").val(parseFloat(result.C_DabtsOfMonth)); //月应还债务
                            $("#ControgAssetValuation_ctl").val(parseFloat(result.C_AssetValuation));//客户资产估值
                            $("#ControgRepayLoan_ctl").val(parseFloat(result.C_RepayLoan)); //客户月还贷能力
                            //alert("月收入估值：" + C_IncomOfMonth + "月应还债务:" + C_DabtsOfMonth + "客户资产估值:" + C_AssetValuation + "客户月还贷能力" + C_RepayLoan);
		            
                        }

                    },
                    //error: function (msg) {
                    //    alert("出错了");
                    //},
                    error: function (msg) {// 19.7 
					    showJqErr(msg);
                    }
                });
            }


        }
    </script>
</asp:Content>
