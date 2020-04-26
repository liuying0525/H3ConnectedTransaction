<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCompanyAppTEST.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SCompanyAppTEST" EnableEventValidation="false" MasterPageFile="~/MvcSheetTEST1.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="../../../jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <script src="../../../jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
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

        .rightbox1 .comment > div {
            padding-left: 50px!important;
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
                    <label><a href="javascript:void(0);" onclick="rsfkClick()">风控评估模型评估结果</a></label>
                    <label><span id="fkpgjg" style="padding-left: 5px; padding-right: 5px;"></span></label>
                    <label><span style="padding-left: 5px;">信用评分:</span></label>
                    <label><span id="grxypf" style="padding-left: 5px;"></span></label>
                </div>
                <div style="width: 10%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a target="_blank" onclick="nciicClick();" href="javascript:void(0);">NCIIC</a></label>
                </div>
                <div style="width: 15%; line-height: 20px; padding: 5px 0px 5px 0px; float: left; position: relative;">
                    <a class="btn" id="rsmanchk" onclick="rsmanchk()">人工查询</a>
                    <span id="searching" style="position: absolute; white-space: nowrap; padding-top: 5px;"></span>
                </div>
                <div style="width: 10%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;" id="aclickh">
                    <label>PBOC</label>
                    <a href="javascript:void(0);" onclick="pbocClick('Control209','Control210','Control211');" target="_blank">担保人</a>
                    <a href="#" id="aclick" target="_blank" style="display: none;">PBOC</a>
                </div>
                <div style="width: 10%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <a href="javascript:void(0);" id="fjxq" onclick="getDownLoadURL()">附件详情</a>
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
                        <div class="col-md-9">
                            <textarea id="Control383" data-datafield="NBLYB" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                        <div id="control422" class="col-md-1">
                            <input id="Control380" type="checkbox" data-datafield="ccbh" data-type="SheetCheckbox" style="">
                        </div>
                        <div id="title422" class="col-md-2">
                            <span id="Label380" data-type="SheetLabel" data-datafield="ccbh" style="">差错驳回</span>
                        </div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title425" class="leftbox1">
                        <span id="Label382" data-type="SheetLabel" data-datafield="THJL" style="">电调记录</span>
                    </div>
                    <div id="control425" class="rightbox1">
                        <textarea id="Control382" data-datafield="THJL" data-type="SheetRichTextBox" style=""></textarea>
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
                                <select data-datafield="xsjjlyzx" data-type="SheetDropDownList" id="ctl207000"
                                    class="" style="" data-schemacode="shyj1" data-querycode="shyj" data-displayemptyitem="true"
                                    data-filter="csshzt:TYPE2,机构贷款:TYPE1,0:PARENTNAME"
                                    data-datavaluefield="REJECTNAME" data-datatextfield="REJECTNAME">
                                </select>
                            </div>
                            <div id="control494" class="rightbox" style="width: 50%; padding: 5px 0!important;">
                                <select data-datafield="xsjjlyzx1" data-type="SheetDropDownList" id="ctl146665"
                                    class="" style="" data-schemacode="shyj1" data-querycode="shyj"
                                    data-filter="csshzt:TYPE2,机构贷款:TYPE1,xsjjlyzx:PARENTNAME" data-displayemptyitem="true"
                                    data-datavaluefield="REJECTNAME" data-datatextfield="REJECTNAME">
                                </select>
                            </div>
                        </div>
                        <div id="xscsyj" data-datafield="CSYJ" data-type="SheetComment" style="">
                        </div>
                    </div>
                </div>
                <div class="row tableContent" id="xszsyj" style="position:relative;z-index:999;">
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
                <div id="control445" class="col-md-12" style="border-left: none;">
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
            <div>
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
                            <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style=""></select>
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
                        <div class="centerline2"></div>
                        <div id="control19" class="rightbox2">
                            <input id="Control27" type="text" data-datafield="applicant_type" data-type="SheetTextBox" style="">
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
            </div>
            <div data-isretract="true" class="isshowdetail">
                <a href="javascript:void(0);" onclick="hideInfo('divBaseInfo1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label id="Label5" data-en_us="Sheet information">申请公司信息</label>
        </div>
        <div class="divContent" id="divSQR">
            <div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title3481" class="leftbox1">
                            <span id="Label3111" data-type="SheetLabel" data-datafield="companyNamech" style="">公司名称</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control3481" class="rightbox1">
                            <input id="Control3111" type="text" data-datafield="companyNamech" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3461" class="leftbox3">
                            <span id="Label3091" data-type="SheetLabel" data-datafield="organizationCode" style="">组织机构代码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3461" class="rightbox3">
                            <input id="Control3091" type="text" data-datafield="organizationCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title3491" class="leftbox1">
                            <span id="Label3121" data-type="SheetLabel" data-datafield="cbusinesstypeName" style="">企业类型</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control3491" class="rightbox1">
                            <input id="Control3121" type="text" data-datafield="cbusinesstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3471" class="leftbox3">
                            <span id="Label3101" data-type="SheetLabel" data-datafield="CregistrationNo" style="">注册号</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3471" class="rightbox3">
                            <input id="Control3101" type="text" data-datafield="CregistrationNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title83" class="leftbox1">
                            <span id="Label85" data-type="SheetLabel" data-datafield="currentLivingAddress" style="">地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control83" class="rightbox1">
                            <textarea id="Control185" data-datafield="currentLivingAddress" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3501" class="leftbox3">
                            <span id="Label3131" data-type="SheetLabel" data-datafield="capitalReamount" style="">注册资本金额</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3501" class="rightbox3">
                            <input id="Control3131" type="text" data-datafield="capitalReamount" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title85" class="leftbox1">
                            <span id="Label86" data-type="SheetLabel" data-datafield="AddressId" style="">注册地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control85" class="rightbox1">
                            <textarea id="Control86" data-datafield="AddressId" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3541" class="leftbox3">
                            <span id="Label3171" data-type="SheetLabel" data-datafield="cindustrytypeName" style="">行业类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3541" class="rightbox3">
                            <input id="Control3171" type="text" data-datafield="cindustrytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title3561" class="leftbox">
                            <span id="Label3191" data-type="SheetLabel" data-datafield="representativeName" style="">法人姓名</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3561" class="rightbox">
                            <input id="Control3191" type="text" data-datafield="representativeName" data-type="SheetTextBox" style="">
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
                        <div id="title3511" class="leftbox">
                            <span id="Label3141" data-type="SheetLabel" data-datafield="establishedin" style="">成立自</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3511" class="rightbox">
                            <input id="Control3141" type="text" data-datafield="establishedin" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                     <div class="col-md-4">
                        <div id="title98" class="leftbox2">
                            <span id="Label98" data-type="SheetLabel" data-datafield="propertytypeName" style="">房产类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control98" class="rightbox2">
                            <input id="Text2" type="text" data-datafield="propertytypeName" data-type="SheetTextBox" style="">
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
                        <div id="title3631" class="leftbox">
                            <span id="Label3261" data-type="SheetLabel" data-datafield="businessHistory" style="">公司年限</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3631" class="rightbox">
                            <input id="Control3261" type="text" data-datafield="businessHistory" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3521" class="leftbox2">
                            <span id="Label3151" data-type="SheetLabel" data-datafield="parentCompany" style="">母公司</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control3521" class="rightbox2">
                            <input id="Control3151" type="text" data-datafield="parentCompany" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3531" class="leftbox3">
                            <span id="Label3161" data-type="SheetLabel" data-datafield="subsidaryCompany" style="">子公司</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3531" class="rightbox3">
                            <input id="Control3161" type="text" data-datafield="subsidaryCompany" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title3571" class="leftbox">
                            <span id="Label3201" data-type="SheetLabel" data-datafield="representativeidNo" style="">法人身份证件号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3571" class="rightbox">
                            <input id="Control3201" type="text" data-datafield="representativeidNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3581" class="leftbox2">
                            <span id="Label3211" data-type="SheetLabel" data-datafield="representativeDesignation" style="">法人职务</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control3581" class="rightbox2">
                            <input id="Control3211" type="text" data-datafield="representativeDesignation" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3591" class="leftbox3">
                            <span id="Label3221" data-type="SheetLabel" data-datafield="loanCard" style="">贷款卡号</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3591" class="rightbox3">
                            <input id="Control3221" type="text" data-datafield="loanCard" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title360" class="leftbox">
                            <span id="Label3231" data-type="SheetLabel" data-datafield="loancardPassword" style="">贷款卡密码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control360" class="rightbox">
                            <input id="Control3231" type="text" data-datafield="loancardPassword" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3611" class="leftbox2">
                            <span id="Label3241" data-type="SheetLabel" data-datafield="compaynameeng" style="">公司名称(英文)</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control3611" class="rightbox2">
                            <input id="Control3241" type="text" data-datafield="compaynameeng" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title362" class="leftbox3">
                            <span id="Label3251" data-type="SheetLabel" data-datafield="taxid" style="">税号</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control362" class="rightbox3">
                            <input id="Control3251" type="text" data-datafield="taxid" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title364" class="leftbox">
                            <span id="Label3271" data-type="SheetLabel" data-datafield="trustName" style="">信托机构名称</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control364" class="rightbox">
                            <input id="Control3271" type="text" data-datafield="trustName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3651" class="leftbox2">
                            <span id="Label3281" data-type="SheetLabel" data-datafield="annualRevenue" style="">收入</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control3651" class="rightbox2">
                            <input id="Control3281" type="text" data-datafield="annualRevenue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title366" class="leftbox3">
                            <span id="Label3291" data-type="SheetLabel" data-datafield="networthamt" style="">净资产额</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control366" class="rightbox3">
                            <input id="Control3291" type="text" data-datafield="networthamt" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title3671" class="leftbox">
                            <span id="Label3301" data-type="SheetLabel" data-datafield="years" style="">年份</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3671" class="rightbox">
                            <input id="Control3301" type="text" data-datafield="years" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title368" class="leftbox2">
                            <span id="Label3311" data-type="SheetLabel" data-datafield="cemailAddress" style="">邮箱地址</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control368" class="rightbox2">
                            <input id="Control3311" type="text" data-datafield="cemailAddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title62" class="leftbox3">
                            <span id="Label68" data-type="SheetLabel" data-datafield="BlacklistInd" style="">外部信用记录</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control62" class="rightbox3">
                            <input id="Control68" type="text" data-datafield="BlacklistInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title61" class="leftbox">
                            <span id="Label67" data-type="SheetLabel" data-datafield="BlackListNoRecordInd" style="">黑名单无记录</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control61" class="rightbox">
                            <input id="Control67" type="text" data-datafield="BlackListNoRecordInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title87" class="leftbox2">
                            <span id="Label87" data-type="SheetLabel" data-datafield="defaultmailingaddress" style="">默认邮寄地址</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control87" class="rightbox2">
                            <input id="Control87" type="text" data-datafield="defaultmailingaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title88" class="leftbox3">
                            <span id="Label88" data-type="SheetLabel" data-datafield="hukouaddress" style="">办公地址</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control88" class="rightbox3">
                            <input id="Control88" type="text" data-datafield="hukouaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title89" class="leftbox">
                            <span id="Label89" data-type="SheetLabel" data-datafield="countryName" style="">国家</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control89" class="rightbox">
                            <input id="Control89" type="text" data-datafield="countryName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title95" class="leftbox2">
                            <span id="Label95" data-type="SheetLabel" data-datafield="postcode" style="">邮编</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control95" class="rightbox2">
                            <input id="Control95" type="text" data-datafield="postcode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title96" class="leftbox3">
                            <span id="Label96" data-type="SheetLabel" data-datafield="addresstypeName" style="">地址类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control96" class="rightbox3">
                            <input id="Control96" type="text" data-datafield="addresstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title97" class="leftbox">
                            <span id="Label97" data-type="SheetLabel" data-datafield="addressstatusName" style="">地址状态</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control97" class="rightbox">
                            <input id="Control97" type="text" data-datafield="addressstatusName" data-type="SheetTextBox" style="">
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
                        <div id="title92" class="leftbox">
                            <span id="Label92" data-type="SheetLabel" data-datafield="hukouprovinceName" style="">省份</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control92" class="rightbox">
                            <input id="Control192" type="text" data-datafield="hukouprovinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title93" class="leftbox2">
                            <span id="Label93" data-type="SheetLabel" data-datafield="hukoucityName" style="">城市</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control93" class="rightbox2">
                            <input id="Text1" type="text" data-datafield="hukoucityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                     <div class="col-md-4">
                        <div id="title3551" class="leftbox3">
                            <span id="Label3181" data-type="SheetLabel" data-datafield="cindustrysubtypeName" style="">行业子类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3551" class="rightbox3">
                            <input id="Control3181" type="text" data-datafield="cindustrysubtypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" class="isshowdetail">
                <a href="javascript:void(0);" onclick="hideInfo('divSQR1',this)">收起 &and;</a>
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
                            <span id="Label209" data-type="SheetLabel" data-datafield="DbThaiFirstName" style="">姓名</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control217" class="rightbox">
                            <input id="Control209" type="text" data-datafield="DbThaiFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title219" class="leftbox2">
                            <span id="Label211" data-type="SheetLabel" data-datafield="DbIdCardNo" style="">担保人证件号码</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control219" class="rightbox2">
                            <input id="Control211" type="text" data-datafield="DbIdCardNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title301" class="leftbox3">
                            <span id="Label285" data-type="SheetLabel" data-datafield="DbphoneNo" style="">担保人电话</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control301" class="rightbox3">
                            <input id="Control285" type="text" data-datafield="DbphoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title221" class="leftbox">
                            <span id="Label213" data-type="SheetLabel" data-datafield="DbgenderName" style="">性别</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control221" class="rightbox">
                            <input id="Control213" type="text" data-datafield="DbgenderName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title258" class="leftbox2">
                            <span id="Label248" data-type="SheetLabel" data-datafield="Dblienee" style="">抵押人</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control258" class="rightbox2">
                            <input id="Control248" type="text" data-datafield="Dblienee" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title223" class="leftbox3">
                            <span id="Label214" data-type="SheetLabel" data-datafield="DbHokouName" style="">户口所在地</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control223" class="rightbox3">
                            <textarea id="Control214" data-datafield="DbHokouName" data-type="SheetTextBox" style=""></textarea>
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
                        <div id="title151" class="leftbox2">
                            <span id="Label235" data-type="SheetLabel" data-datafield="DbEducationName" style="">教育程度 </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control151" class="rightbox2">
                            <input id="Control235" type="text" data-datafield="DbEducationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox3">第三方黑名单无记录</div>
                        <div class="centerline3"></div>
                        <div id="dbrhmd" class="rightbox3">
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
                        <div class="leftbox3">第三方外部信用记录</div>
                        <div class="centerline3"></div>
                        <div id="dbrxyjl" class="rightbox3">
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
                            <textarea id="Control265" data-datafield="DbcurrentLivingAddress" data-type="SheetRichTextBox" style=""></textarea>
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
                    <div class="col-md-8">
                        <div id="title281" class="leftbox1">
                            <span id="Label266" data-type="SheetLabel" data-datafield="DbAddressId" style="">户籍地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control281" class="rightbox1">
                            <textarea id="Control266" data-datafield="DbAddressId" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title286" class="leftbox3">
                            <span id="Label270" data-type="SheetLabel" data-datafield="Dbnativedistrict" style="">籍贯</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control286" class="rightbox3">
                            <input id="Control270" type="text" data-datafield="Dbnativedistrict" data-type="SheetTextBox" style="">
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
                        <div id="title288" class="leftbox3">
                            <span id="Label272" data-type="SheetLabel" data-datafield="DbhukouprovinceName" style="">户籍省份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control288" class="rightbox3">
                            <input id="Control272" type="text" data-datafield="DbhukouprovinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title263" class="leftbox1">
                            <span id="Label251" data-type="SheetLabel" data-datafield="DbBusinessTypeName" style="">企业性质</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control263" class="rightbox1">
                            <input id="Control251" type="text" data-datafield="DbBusinessTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title267" class="leftbox3">
                            <span id="Label255" data-type="SheetLabel" data-datafield="Dbphonenumber" style="">公司电话</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control267" class="rightbox3">
                            <input id="Control255" type="text" data-datafield="Dbphonenumber" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div id="divDBR2">
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
                        <div id="title226" class="leftbox2">
                            <span id="Label216" data-type="SheetLabel" data-datafield="DbCitizenshipName" style="">担保人国籍</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control226" class="rightbox2">
                            <input id="Control216" type="text" data-datafield="DbCitizenshipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title227" class="leftbox3">
                            <span id="Label217" data-type="SheetLabel" data-datafield="DbIdCardIssueDate" style="">担保人证件发行日</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control227" class="rightbox3">
                            <input id="Control217" type="text" data-datafield="DbIdCardIssueDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title228" class="leftbox">
                            <span id="Label218" data-type="SheetLabel" data-datafield="DbIdCardExpiryDate" style="">担保人证件到期日</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control228" class="rightbox">
                            <input id="Control218" type="text" data-datafield="DbIdCardExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title229" class="leftbox2">
                            <span id="Label219" data-type="SheetLabel" data-datafield="DbLicenseNo" style="">担保人驾照号码</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control229" class="rightbox2">
                            <input id="Control219" type="text" data-datafield="DbLicenseNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title230" class="leftbox3">
                            <span id="Label220" data-type="SheetLabel" data-datafield="DbLicenseExpiryDate" style="">担保人驾照到期日</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control230" class="rightbox3">
                            <input id="Control220" type="text" data-datafield="DbLicenseExpiryDate" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title231" class="leftbox">
                            <span id="Label221" data-type="SheetLabel" data-datafield="DbThaiTitleName" style="">担保人头衔</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control231" class="rightbox">
                            <input id="Control221" type="text" data-datafield="DbThaiTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title232" class="leftbox2">
                            <span id="Label222" data-type="SheetLabel" data-datafield="DbTitleName" style="">担保人头衔（英文）</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control232" class="rightbox2">
                            <input id="Control222" type="text" data-datafield="DbTitleName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title234" class="leftbox3">
                            <span id="Label224" data-type="SheetLabel" data-datafield="DbEngFirstName" style="">担保人名（英文）</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control234" class="rightbox3">
                            <input id="Control224" type="text" data-datafield="DbEngFirstName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title235" class="leftbox">
                            <span id="Label225" data-type="SheetLabel" data-datafield="DbEngLastName" style="">担保人中间名字</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control235" class="rightbox">
                            <input id="Control225" type="text" data-datafield="DbEngLastName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title236" class="leftbox2">
                            <span id="Label226" data-type="SheetLabel" data-datafield="DbFormerName" style="">担保人曾用名</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control236" class="rightbox2">
                            <input id="Control226" type="text" data-datafield="DbFormerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title237" class="leftbox3">
                            <span id="Label227" data-type="SheetLabel" data-datafield="DbNationCode" style="">担保人民族</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control237" class="rightbox3">
                            <input id="Control227" type="text" data-datafield="DbNationCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title238" class="leftbox">
                            <span id="Label228" data-type="SheetLabel" data-datafield="DbIssuingAuthority" style="">担保人签发机关</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control238" class="rightbox">
                            <input id="Control228" type="text" data-datafield="DbIssuingAuthority" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title239" class="leftbox2">
                            <span id="Label229" data-type="SheetLabel" data-datafield="DbNumberOfDependents" style="">担保人供养人数</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control239" class="rightbox2">
                            <input id="Control229" type="text" data-datafield="DbNumberOfDependents" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title240" class="leftbox3">
                            <span id="Label230" data-type="SheetLabel" data-datafield="DbHouseOwnerName" style="">担保人房产所有人</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control240" class="rightbox3">
                            <input id="Control230" type="text" data-datafield="DbHouseOwnerName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title241" class="leftbox">
                            <span id="Label231" data-type="SheetLabel" data-datafield="DbNoOfFamilyMembers" style="">担保人家庭人数</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control241" class="rightbox">
                            <input id="Control231" type="text" data-datafield="DbNoOfFamilyMembers" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title242" class="leftbox2">
                            <span id="Label232" data-type="SheetLabel" data-datafield="DbChildrenFlag" style="">担保人是否是子女</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control242" class="rightbox2">
                            <input id="Control232" type="text" data-datafield="DbChildrenFlag" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title243" class="leftbox3">
                            <span id="Label233" data-type="SheetLabel" data-datafield="DbEmploymentTypeName" style="">担保人雇员类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control243" class="rightbox3">
                            <input id="Control233" type="text" data-datafield="DbEmploymentTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title244" class="leftbox">
                            <span id="Label234" data-type="SheetLabel" data-datafield="DbEmailAddress" style="">担保人邮箱地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control244" class="rightbox">
                            <input id="Control234" type="text" data-datafield="DbEmailAddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title246" class="leftbox2">
                            <span id="Label236" data-type="SheetLabel" data-datafield="DbIndustryTypeName" style="">担保人行业类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control246" class="rightbox2">
                            <input id="Control236" type="text" data-datafield="DbIndustryTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title247" class="leftbox3">
                            <span id="Label237" data-type="SheetLabel" data-datafield="DbIndustrySubTypeName" style="">担保人行业子类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control247" class="rightbox3">
                            <input id="Control237" type="text" data-datafield="DbIndustrySubTypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title248" class="leftbox">
                            <span id="Label238" data-type="SheetLabel" data-datafield="DbOccupationName" style="">担保人职业类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control248" class="rightbox">
                            <input id="Control238" type="text" data-datafield="DbOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title249" class="leftbox2">
                            <span id="Label239" data-type="SheetLabel" data-datafield="DbSubOccupationName" style="">担保人职业子类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control249" class="rightbox2">
                            <input id="Control239" type="text" data-datafield="DbSubOccupationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title250" class="leftbox3">
                            <span id="Label240" data-type="SheetLabel" data-datafield="DbDesginationName" style="">担保人职位</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control250" class="rightbox3">
                            <input id="Control240" type="text" data-datafield="DbDesginationName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title251" class="leftbox">
                            <span id="Label241" data-type="SheetLabel" data-datafield="DbJobGroupName" style="">担保人工作组</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control251" class="rightbox">
                            <input id="Control241" type="text" data-datafield="DbJobGroupName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title252" class="leftbox2">
                            <span id="Label242" data-type="SheetLabel" data-datafield="DbSalaryRangeName" style="">担保人估计收入</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control252" class="rightbox2">
                            <input id="Control242" type="text" data-datafield="DbSalaryRangeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title253" class="leftbox3">
                            <span id="Label243" data-type="SheetLabel" data-datafield="DbConsent" style="">担保人同意</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control253" class="rightbox3">
                            <input id="Control243" type="text" data-datafield="DbConsent" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title254" class="leftbox">
                            <span id="Label244" data-type="SheetLabel" data-datafield="DbVIPInd" style="">担保人贵宾</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control254" class="rightbox">
                            <input id="Control244" type="text" data-datafield="DbVIPInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title255" class="leftbox2">
                            <span id="Label245" data-type="SheetLabel" data-datafield="DbStaffInd" style="">担保人工作人员</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control255" class="rightbox2">
                            <input id="Control245" type="text" data-datafield="DbStaffInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title256" class="leftbox2">
                            <span id="Label246" data-type="SheetLabel" data-datafield="DbBlackListNoRecordInd" style="">担保人黑名单无记录</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control256" class="rightbox3">
                            <input id="Control246" type="text" data-datafield="DbBlackListNoRecordInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title257" class="leftbox">
                            <span id="Label247" data-type="SheetLabel" data-datafield="DbBlacklistInd" style="">担保人外部信用记录</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control257" class="rightbox">
                            <input id="Control247" type="text" data-datafield="DbBlacklistInd" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title264" class="leftbox2">
                            <span id="Label252" data-type="SheetLabel" data-datafield="DbpositionName" style="">担保人职位</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control264" class="rightbox2">
                            <input id="Control252" type="text" data-datafield="DbpositionName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title265" class="leftbox3">
                            <span id="Label253" data-type="SheetLabel" data-datafield="DbcompanyProvinceName" style="">担保人公司省份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control265" class="rightbox3">
                            <input id="Control253" type="text" data-datafield="DbcompanyProvinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title268" class="leftbox">
                            <span id="Label256" data-type="SheetLabel" data-datafield="DbFax" style="">担保人公司传真 </span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control268" class="rightbox">
                            <input id="Control256" type="text" data-datafield="DbFax" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-8">
                        <div id="title269" class="leftbox1">
                            <span id="Label257" data-type="SheetLabel" data-datafield="DbcompanyAddress" style="">担保人公司地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control269" class="rightbox1">
                            <textarea id="Control257" data-datafield="DbcompanyAddress" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title271" class="leftbox">
                            <span id="Label258" data-type="SheetLabel" data-datafield="DbcompanypostCode" style="">担保人公司邮编 </span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control271" class="rightbox">
                            <input id="Control258" type="text" data-datafield="DbcompanypostCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title272" class="leftbox2">
                            <span id="Label259" data-type="SheetLabel" data-datafield="DbEmployerType" style="">担保人雇主类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control272" class="rightbox2">
                            <input id="Control259" type="text" data-datafield="DbEmployerType" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title273" class="leftbox3">
                            <span id="Label260" data-type="SheetLabel" data-datafield="DbNoOfEmployees" style="">担保人雇员人数</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control273" class="rightbox3">
                            <input id="Control260" type="text" data-datafield="DbNoOfEmployees" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title274" class="leftbox">
                            <span id="Label261" data-type="SheetLabel" data-datafield="DbJobDescription" style="">担保人工作描述</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control274" class="rightbox">
                            <input id="Control261" type="text" data-datafield="DbJobDescription" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title276" class="leftbox2">
                            <span id="Label263" data-type="SheetLabel" data-datafield="Dbtimeinmonth" style="">担保人工作年限（月） </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control276" class="rightbox2">
                            <input id="Control263" type="text" data-datafield="Dbtimeinmonth" data-type="SheetTextBox" style="">
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
                        <div id="title284" class="leftbox3">
                            <span id="Label268" data-type="SheetLabel" data-datafield="Dbhukouaddress" style="">担保人户籍地址</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control284" class="rightbox3">
                            <input id="Control268" type="text" data-datafield="Dbhukouaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title285" class="leftbox">
                            <span id="Label269" data-type="SheetLabel" data-datafield="DbcountryName" style="">担保人国家</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control285" class="rightbox">
                            <input id="Control269" type="text" data-datafield="DbcountryName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title287" class="leftbox2">
                            <span id="Label271" data-type="SheetLabel" data-datafield="Dbbirthpalaceprovince" style="">担保人出生地省市县（区） </span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control287" class="rightbox2">
                            <input id="Control271" type="text" data-datafield="Dbbirthpalaceprovince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title289" class="leftbox3">
                            <span id="Label273" data-type="SheetLabel" data-datafield="DbhukoucityName" style="">担保人城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control289" class="rightbox3">
                            <input id="Control273" type="text" data-datafield="DbhukoucityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title290" class="leftbox">
                            <span id="Label274" data-type="SheetLabel" data-datafield="Dbpostcode" style="">担保人邮编</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control290" class="rightbox">
                            <input id="Control274" type="text" data-datafield="Dbpostcode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title291" class="leftbox2">
                            <span id="Label275" data-type="SheetLabel" data-datafield="DbaddresstypeName" style="">担保人地址类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control291" class="rightbox2">
                            <input id="Control275" type="text" data-datafield="DbaddresstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title292" class="leftbox3">
                            <span id="Label276" data-type="SheetLabel" data-datafield="DbaddressstatusName" style="">担保人地址状态</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control292" class="rightbox3">
                            <input id="Control276" type="text" data-datafield="DbaddressstatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title293" class="leftbox">
                            <span id="Label277" data-type="SheetLabel" data-datafield="DbpropertytypeName" style="">担保人房产类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control293" class="rightbox">
                            <input id="Control277" type="text" data-datafield="DbpropertytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title294" class="leftbox2">
                            <span id="Label278" data-type="SheetLabel" data-datafield="DbresidencetypeName" style="">担保人住宅类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control294" class="rightbox2">
                            <input id="Control278" type="text" data-datafield="DbresidencetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title295" class="leftbox3">
                            <span id="Label279" data-type="SheetLabel" data-datafield="Dblivingsince" style="">担保人开始居住日期</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control295" class="rightbox3">
                            <input id="Control279" type="text" data-datafield="Dblivingsince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title296" class="leftbox">
                            <span id="Label280" data-type="SheetLabel" data-datafield="Dbhomevalue" style="">担保人房屋价值（万元) </span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control296" class="rightbox">
                            <input id="Control280" type="text" data-datafield="Dbhomevalue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title297" class="leftbox2">
                            <span id="Label281" data-type="SheetLabel" data-datafield="Dbstayinyear" style="">担保人居住年限</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control297" class="rightbox2">
                            <input id="Control281" type="text" data-datafield="Dbstayinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title298" class="leftbox3">
                            <span id="Label282" data-type="SheetLabel" data-datafield="Dbstayinmonth" style="">担保人居住月份</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control298" class="rightbox3">
                            <input id="Control282" type="text" data-datafield="Dbstayinmonth" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title299" class="leftbox">
                            <span id="Label283" data-type="SheetLabel" data-datafield="DbcountrycodeName" style="">担保人国家代码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control299" class="rightbox">
                            <input id="Control283" type="text" data-datafield="DbcountrycodeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title300" class="leftbox2">
                            <span id="Label284" data-type="SheetLabel" data-datafield="DbareaCode" style="">担保人地区代码</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control300" class="rightbox2">
                            <input id="Control284" type="text" data-datafield="DbareaCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title302" class="leftbox3">
                            <span id="Label286" data-type="SheetLabel" data-datafield="Dbextension" style="">担保人分机</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control302" class="rightbox3">
                            <input id="Control286" type="text" data-datafield="Dbextension" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title303" class="leftbox">
                            <span id="Label287" data-type="SheetLabel" data-datafield="DbphonetypeName" style="">担保人电话类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control303" class="rightbox">
                            <input id="Control287" type="text" data-datafield="DbphonetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title113" class="leftbox2">
                            <span id="Label113" data-type="SheetLabel" data-datafield="name1" style="">名称</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control113" class="rightbox2">
                            <input id="Control113" type="text" data-datafield="name1" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title114" class="leftbox3">
                            <span id="Label114" data-type="SheetLabel" data-datafield="relationshipName" style="">关系</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control114" class="rightbox3">
                            <input id="Control114" type="text" data-datafield="relationshipName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title115" class="leftbox">
                            <span id="Label115" data-type="SheetLabel" data-datafield="address" style="">联系人地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control115" class="rightbox">
                            <input id="Control115" type="text" data-datafield="address" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title116" class="leftbox2">
                            <span id="Label116" data-type="SheetLabel" data-datafield="lxprovinceCode" style="">联系人省份</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control116" class="rightbox2">
                            <input id="Control116" type="text" data-datafield="lxprovinceCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title117" class="leftbox3">
                            <span id="Label117" data-type="SheetLabel" data-datafield="lxcityName" style="">联系人城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control117" class="rightbox3">
                            <input id="Control117" type="text" data-datafield="lxcityName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title118" class="leftbox">
                            <span id="Label118" data-type="SheetLabel" data-datafield="lxpostCode" style="">联系人邮编</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control118" class="rightbox">
                            <input id="Control118" type="text" data-datafield="lxpostCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title121" class="leftbox2">
                            <span id="Label120" data-type="SheetLabel" data-datafield="lxphoneNo" style="">联系人电话</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control121" class="rightbox2">
                            <input id="Control120" type="text" data-datafield="lxphoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title122" class="leftbox3">
                            <span id="Label121" data-type="SheetLabel" data-datafield="lxmobile" style="">联系人手机</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control122" class="rightbox3">
                            <input id="Control121" type="text" data-datafield="lxmobile" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title119" class="leftbox1">
                            <span id="Label119" data-type="SheetLabel" data-datafield="lxhokouName" style="">联系人户口所在地</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control119" class="rightbox1">
                            <textarea id="Control119" data-datafield="lxhokouName" data-type="SheetTextBox" style=""></textarea>
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
                <div class="row">
                    <div class="col-md-4">
                        <div id="title275" class="leftbox">
                            <span id="Label262" data-type="SheetLabel" data-datafield="Dbtimeinyear" style="">工作年限（年）</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control275" class="rightbox">
                            <input id="Control262" type="text" data-datafield="Dbtimeinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title259" class="leftbox2">
                            <span id="Label249" data-type="SheetLabel" data-datafield="DbActualSalary" style="">月收入</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control259" class="rightbox2">
                            <input id="Control249" type="text" data-datafield="DbActualSalary" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" class="isshowdetail">
                <a href="javascript:void(0);" onclick="hideInfo('divDBR2',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('divDBR1',this)">
            <label id="Label711" data-en_us="Sheet information">担保公司信息</label>
        </div>
        <div class="divContent" id="divDBR1">
            <div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title3712" class="leftbox1">
                            <span id="Label3342" data-type="SheetLabel" data-datafield="DbCcompanyNamech" style="">公司名称</span>
                        </div>
                        <div class="centerline" style="left: 17.5%"></div>
                        <div id="control3712" class="rightbox1">
                            <input id="Control3342" type="text" data-datafield="DbCcompanyNamech" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3692" class="leftbox3">
                            <span id="Label3322" data-type="SheetLabel" data-datafield="DbCorganizationCode" style="">组织机构代码</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3692" class="rightbox3">
                            <input id="Control3322" type="text" data-datafield="DbCorganizationCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title372" class="leftbox1">
                            <span id="Label3352" data-type="SheetLabel" data-datafield="DbCbusinesstypeName" style="">企业类型</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control372" class="rightbox1">
                            <input id="Control3352" type="text" data-datafield="DbCbusinesstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title370" class="leftbox3">
                            <span id="Label3332" data-type="SheetLabel" data-datafield="DbCregistrationNo" style="">注册号</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control370" class="rightbox3">
                            <input id="Control3332" type="text" data-datafield="DbCregistrationNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title395" class="leftbox1">
                            <span id="Label3571" data-type="SheetLabel" data-datafield="DbCcurrentLivingAddress" style="">地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control395" class="rightbox1">
                            <textarea id="Control3571" data-datafield="DbCcurrentLivingAddress" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3732" class="leftbox3">
                            <span id="Label3362" data-type="SheetLabel" data-datafield="DbCcapitalReamount" style="">注册资本金额</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control3732" class="rightbox3">
                            <input id="Control3362" type="text" data-datafield="DbCcapitalReamount" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div id="title397" class="leftbox1">
                            <span id="Label358" data-type="SheetLabel" data-datafield="DbCAddressId" style="">注册地址</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control397" class="rightbox1">
                            <textarea id="Control1358" data-datafield="DbCAddressId" data-type="SheetRichTextBox" style=""></textarea>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title377" class="leftbox3">
                            <span id="Label3402" data-type="SheetLabel" data-datafield="DbCindustrytypeName" style="">行业类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control377" class="rightbox3">
                            <input id="Control3402" type="text" data-datafield="DbCindustrytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title379" class="leftbox">
                            <span id="Label3422" data-type="SheetLabel" data-datafield="DbCrepresentativeName" style="">法人姓名</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control379" class="rightbox">
                            <input id="Control3422" type="text" data-datafield="DbCrepresentativeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title413" class="leftbox2">
                            <span id="Label373" data-type="SheetLabel" data-datafield="DbCphoneNo" style="">电话</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control413" class="rightbox2">
                            <input id="Control373" type="text" data-datafield="DbCphoneNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title393" class="leftbox3">
                            <span id="Label356" data-type="SheetLabel" data-datafield="DbClienee" style="">抵押人</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control393" class="rightbox3">
                            <input id="Control356" type="text" data-datafield="DbClienee" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title374" class="leftbox">
                            <span id="Label3372" data-type="SheetLabel" data-datafield="DbCestablishedin" style="">成立自</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control374" class="rightbox">
                            <input id="Control3372" type="text" data-datafield="DbCestablishedin" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title387" class="leftbox2">
                            <span id="Label350" data-type="SheetLabel" data-datafield="DbCbusinessHistory" style="">公司年限</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control387" class="rightbox2">
                            <input id="Control350" type="text" data-datafield="DbCbusinessHistory" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title407" class="leftbox3">
                            <span id="Label367" data-type="SheetLabel" data-datafield="DbCpropertytypeName" style="">房产类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control407" class="rightbox3">
                            <input id="Control367" type="text" data-datafield="DbCpropertytypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div id="divDBR11">
                <div class="row">
                    <div class="col-md-4">
                        <div id="title3752" class="leftbox">
                            <span id="Label3382" data-type="SheetLabel" data-datafield="DbCparentCompany" style="">担保公司母公司</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3752" class="rightbox">
                            <input id="Control3382" type="text" data-datafield="DbCparentCompany" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title376" class="leftbox2">
                            <span id="Label33922" data-type="SheetLabel" data-datafield="DbCsubsidaryCompany" style="">担保公司子公司</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control376" class="rightbox2">
                            <input id="Control3392" type="text" data-datafield="DbCsubsidaryCompany" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title378" class="leftbox3">
                            <span id="Label3412" data-type="SheetLabel" data-datafield="DbCindustrysubtypeName" style="">担保公司行业子类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control378" class="rightbox3">
                            <input id="Control3412" type="text" data-datafield="DbCindustrysubtypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title380" class="leftbox">
                            <span id="Label3432" data-type="SheetLabel" data-datafield="DbCrepresentativeidNo" style="">担保公司法人身份证件号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control380" class="rightbox">
                            <input id="Control3432" type="text" data-datafield="DbCrepresentativeidNo" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title381" class="leftbox2">
                            <span id="Label3442" data-type="SheetLabel" data-datafield="DbCrepresentativeDesignation" style="">担保公司法人职务</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control381" class="rightbox2">
                            <input id="Control3442" type="text" data-datafield="DbCrepresentativeDesignation" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title382" class="leftbox3">
                            <span id="Label3452" data-type="SheetLabel" data-datafield="DbCloanCard" style="">担保公司贷款卡号</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control382" class="rightbox3">
                            <input id="Control3452" type="text" data-datafield="DbCloanCard" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title383" class="leftbox">
                            <span id="Label3462" data-type="SheetLabel" data-datafield="DbCloancardPassword" style="">担保公司贷款卡密码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control383" class="rightbox">
                            <input id="Control3462" type="text" data-datafield="DbCloancardPassword" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title384" class="leftbox2">
                            <span id="Label3472" data-type="SheetLabel" data-datafield="DbCcomments" style="">担保公司评论</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control384" class="rightbox2">
                            <input id="Control3472" type="text" data-datafield="DbCcomments" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title385" class="leftbox3">
                            <span id="Label3482" data-type="SheetLabel" data-datafield="DbCcompaynameeng" style="">担保公司公司名称(英文)</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control385" class="rightbox3">
                            <input id="Control3482" type="text" data-datafield="DbCcompaynameeng" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title386" class="leftbox">
                            <span id="Label3492" data-type="SheetLabel" data-datafield="DbCtaxid" style="">担保公司税号</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control386" class="rightbox">
                            <input id="Control3492" type="text" data-datafield="DbCtaxid" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title388" class="leftbox2">
                            <span id="Label351" data-type="SheetLabel" data-datafield="DbCtrustName" style="">担保公司信托机构名称</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control388" class="rightbox2">
                            <input id="Control351" type="text" data-datafield="DbCtrustName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title389" class="leftbox3">
                            <span id="Label352" data-type="SheetLabel" data-datafield="DbCannualRevenue" style="">担保公司收入</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control389" class="rightbox3">
                            <input id="Control352" type="text" data-datafield="DbCannualRevenue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title390" class="leftbox">
                            <span id="Label353" data-type="SheetLabel" data-datafield="DbCnetworthamt" style="">担保公司净资产额</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control390" class="rightbox">
                            <input id="Control353" type="text" data-datafield="DbCnetworthamt" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title3911" class="leftbox2">
                            <span id="Label354" data-type="SheetLabel" data-datafield="DbCyears" style="">担保公司年份</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control3911" class="rightbox2">
                            <input id="Control354" type="text" data-datafield="DbCyears" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title392" class="leftbox3">
                            <span id="Label355" data-type="SheetLabel" data-datafield="DbCemailAddress" style="">担保公司邮箱地址</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control392" class="rightbox3">
                            <input id="Control355" type="text" data-datafield="DbCemailAddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title3991" class="leftbox">
                            <span id="Label359" data-type="SheetLabel" data-datafield="DbCdefaultmailingaddress" style="">担保公司默认邮寄地址</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control3991" class="rightbox">
                            <input id="Control359" type="text" data-datafield="DbCdefaultmailingaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title400" class="leftbox2">
                            <span id="Label360" data-type="SheetLabel" data-datafield="DbChukouaddress" style="">担保公司户籍地址</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control400" class="rightbox2">
                            <input id="Control360" type="text" data-datafield="DbChukouaddress" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title4011" class="leftbox3">
                            <span id="Label3611" data-type="SheetLabel" data-datafield="DbCcountryName" style="">担保公司国家</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control4011" class="rightbox3">
                            <input id="Control3611" type="text" data-datafield="DbCcountryName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title403" class="leftbox">
                            <span id="Label363" data-type="SheetLabel" data-datafield="DbCpostcode" style="">担保公司邮编</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control403" class="rightbox">
                            <input id="Control363" type="text" data-datafield="DbCpostcode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title405" class="leftbox2">
                            <span id="Label365" data-type="SheetLabel" data-datafield="DbCaddresstypeName" style="">担保公司地址类型</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control405" class="rightbox2">
                            <input id="Control365" type="text" data-datafield="DbCaddresstypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title406" class="leftbox3">
                            <span id="Label366" data-type="SheetLabel" data-datafield="DbCaddressstatusName" style="">担保公司地址状态</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control406" class="rightbox3">
                            <input id="Control366" type="text" data-datafield="DbCaddressstatusName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title408" class="leftbox">
                            <span id="Label368" data-type="SheetLabel" data-datafield="DbCresidencetypeName" style="">担保公司住宅类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control408" class="rightbox">
                            <input id="Control368" type="text" data-datafield="DbCresidencetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title409" class="leftbox2">
                            <span id="Label369" data-type="SheetLabel" data-datafield="DbClivingsince" style="">担保公司开始居住日期</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control409" class="rightbox2">
                            <input id="Control369" type="text" data-datafield="DbClivingsince" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title410" class="leftbox3">
                            <span id="Label370" data-type="SheetLabel" data-datafield="DbChomevalue" style="">担保公司房屋价值（万元) </span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control410" class="rightbox3">
                            <input id="Control370" type="text" data-datafield="DbChomevalue" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title411" class="leftbox">
                            <span id="Label371" data-type="SheetLabel" data-datafield="DbCstayinyear" style="">担保公司居住年限</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control411" class="rightbox">
                            <input id="Control371" type="text" data-datafield="DbCstayinyear" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title412" class="leftbox2">
                            <span id="Label372" data-type="SheetLabel" data-datafield="DbCareaCode" style="">担保公司地区代码</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control412" class="rightbox2">
                            <input id="Control372" type="text" data-datafield="DbCareaCode" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title414" class="leftbox3">
                            <span id="Label374" data-type="SheetLabel" data-datafield="DbCextension" style="">担保公司分机</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control414" class="rightbox3">
                            <input id="Control374" type="text" data-datafield="DbCextension" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title415" class="leftbox">
                            <span id="Label375" data-type="SheetLabel" data-datafield="DbCphonetypeName" style="">担保公司电话类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control415" class="rightbox">
                            <input id="Control375" type="text" data-datafield="DbCphonetypeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title416" class="leftbox2">
                            <span id="Label376" data-type="SheetLabel" data-datafield="DbCcountrycodeName" style="">担保公司国家代码</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control416" class="rightbox2">
                            <input id="Control376" type="text" data-datafield="DbCcountrycodeName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title404" class="leftbox3">
                            <span id="Label364" data-type="SheetLabel" data-datafield="DbCcityname" style="">城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control404" class="rightbox3">
                            <input id="Control364" type="text" data-datafield="DbCcityname" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title402" class="leftbox">
                            <span id="Label3621" data-type="SheetLabel" data-datafield="DbCprovinceName" style="">省份</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control402" class="rightbox">
                            <input id="Control3621" type="text" data-datafield="DbCprovinceName" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" class="isshowdetail">
                <a href="javascript:void(0);" onclick="hideInfo('divDBR11',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label id="Label3" data-en_us="Basic information">资产信息</label>
        </div>
        <div class="divContent" id="divZC">
            <div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title304" class="leftbox">
                            <span id="Label288" data-type="SheetLabel" data-datafield="assetConditionName" style="">资产状况</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control304" class="rightbox">
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
                        <div id="control309" class="rightbox">
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
                <div class="row">
                    <div class="col-md-8">
                        <div id="title333" class="leftbox1">
                            <span id="Label316" data-type="SheetLabel" data-datafield="vecomments" style="">车辆备注</span>
                        </div>
                        <div class="centerline" style="left: 17.5%;"></div>
                        <div id="control333" class="rightbox1">
                            <textarea id="Control316" data-datafield="vecomments" data-type="SheetRichTextBox" style=""></textarea>
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
            <div data-isretract="true" class="isshowdetail">
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
                            <div id="title245" class="leftbox">
                                <span id="Label321" data-type="SheetLabel" data-datafield="termMonth" style="">贷款期数（月）</span>
                            </div>
                            <div class="centerline"></div>
                            <div id="control245" class="rightbox">
                                <input id="Control321" type="text" data-datafield="termMonth" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="title330" class="leftbox2">
                                <span id="Label314" data-type="SheetLabel" data-datafield="totalaccessoryamount" style="">附加费</span>
                            </div>
                            <div class="centerline2"></div>
                            <div id="control330" class="rightbox2">
                                <input id="Control314" type="text" data-datafield="totalaccessoryamount" data-type="SheetTextBox" style="">
                                 <a href="#accessoryDetail" data-toggle="modal" onclick="" target="_blank">详细</a>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div id="" class="leftbox3">
                                <span>敞口金额</span>
                            </div>
                            <div class="centerline3"></div>
                            <div id="ckje" class="rightbox3">
                                <span id="ctrlCkje" style="color: red;"></span>
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
                                <input id="Control330" type="text" data-datafield="financedamount" data-type="SheetTextBox" style="">
                            </div>
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title419" class="col-md-2">
                            <span id="Label378" data-type="SheetLabel" data-datafield="CompanyDetailtable" style="">还款计划</span>
                        </div>
                        <div id="control419" class="col-md-10">
                            <table id="Table1" data-datafield="CompanyDetailtable" data-type="SheetGridView" class="SheetGridView">
                                <tbody>
                                    <tr class="header">
                                        <td id="Control378_SerialNo" class="rowSerialNo">序号								</td>
                                        <td id="Control378_Header3" data-datafield="CompanyDetailtable.startTerm">
                                            <label id="Control378_Label3" data-datafield="CompanyDetailtable.startTerm" data-type="SheetLabel" style="">还款起始期</label>
                                        </td>
                                        <td id="Control378_Header4" data-datafield="CompanyDetailtable.endTerm">
                                            <label id="Control378_Label4" data-datafield="CompanyDetailtable.endTerm" data-type="SheetLabel" style="">还款结束期</label>
                                        </td>
                                        <td id="Control378_Header5" data-datafield="CompanyDetailtable.rentalAmount">
                                            <label id="Control378_Label5" data-datafield="CompanyDetailtable.rentalAmount" data-type="SheetLabel" style="">还款额</label>
                                        </td>
                                        <td id="Control378_Header6" data-datafield="CompanyDetailtable.grossRentalAmount">
                                            <label id="Control378_Label6" data-datafield="CompanyDetailtable.grossRentalAmount" data-type="SheetLabel" style="">每期还款总额</label>
                                        </td>
                                        <td class="rowOption">删除								</td>
                                    </tr>
                                    <tr class="template">
                                        <td id="Control378_Option" class="rowOption"></td>
                                        <td data-datafield="CompanyDetailtable.startTerm">
                                            <input id="Control378_ctl3" type="text" data-datafield="CompanyDetailtable.startTerm" data-type="SheetTextBox" style="">
                                        </td>
                                        <td data-datafield="CompanyDetailtable.endTerm">
                                            <input id="Control378_ctl4" type="text" data-datafield="CompanyDetailtable.endTerm" data-type="SheetTextBox" style="">
                                        </td>
                                        <td data-datafield="CompanyDetailtable.rentalAmount">
                                            <input id="Control378_ctl5" type="text" data-datafield="CompanyDetailtable.rentalAmount" data-type="SheetTextBox" style="">
                                        </td>
                                        <td data-datafield="CompanyDetailtable.grossRentalAmount">
                                            <input id="Control378_ctl6" type="text" data-datafield="CompanyDetailtable.grossRentalAmount" data-type="SheetTextBox" style="">
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
                                        <td data-datafield="CompanyDetailtable.startTerm"></td>
                                        <td data-datafield="CompanyDetailtable.endTerm"></td>
                                        <td data-datafield="CompanyDetailtable.rentalAmount"></td>
                                        <td data-datafield="CompanyDetailtable.grossRentalAmount"></td>
                                        <td class="rowOption"></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div id="divCPXZ1">
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
                    </div>
                </div>
                <div data-isretract="true" class="isshowdetail">
                    <a href="javascript:void(0);" onclick="hideInfo('divCPXZ1',this)">收起 &and;</a>
                </div>
            </div>
            <div class="nav-icon fa  fa-angle-double-down ss" style="width: 90%; margin-top: 10px; margin-bottom: 10px; border-bottom: 1px solid #ccc;">
                <label data-en_us="Sheet information">资产/负债</label>
            </div>
            <div style="position: relative; padding-right: 10%;" id="divzcfz">
                <div class="row">
                    <div class="col-md-4">
                        <div id="title339" class="leftbox">
                            <span id="Label390" data-type="SheetLabel" data-datafield="Bccdygje" style="">本次车贷月供金额</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control339" class="rightbox">
                            <input id="Control390" type="text" data-datafield="Bccdygje" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title108" class="leftbox2">
                            <span id="Label108" data-type="SheetLabel" data-datafield="Descritption" style="">收入</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control108" class="rightbox2">
                            <input id="Control108" type="text" data-datafield="Descritption" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title109" class="leftbox3">
                            <span id="Label109" data-type="SheetLabel" data-datafield="Descritption2" style="">支出</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control109" class="rightbox3">
                            <input id="Control109" type="text" data-datafield="Descritption2" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title110" class="leftbox">
                            <span id="Label110" data-type="SheetLabel" data-datafield="Value2" style="">金额</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control110" class="rightbox">
                            <input id="Control110" type="text" data-datafield="Value2" data-type="SheetTextBox" style="">
                        </div>
                    </div>
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
                    <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">银行卡</span>
                </div>
                <div id="control391" class="col-md-10">
                    <div id="Control357" data-datafield="YHKMFYJ" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="title435" class="col-md-2">
                    <span id="Label389" data-type="SheetLabel" data-datafield="xsqtfj" style="">信审其他附件</span>
                </div>
                <div id="control435" class="col-md-10">
                    <div id="Control389" data-datafield="xsqtfj" data-type="SheetAttachment" style="">
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
                            <td id="Td1" class="rowSerialNo"></td>
                            <td id="Td2">
                                <label id="Label26" style="">融资额</label>
                            </td>
                            <td id="Td3">
                                <label id="Label6" style="">申请号</label>
                            </td>
                            <td id="Td4">
                                <label id="Label8" style="">状态</label>
                            </td>
                            <td id="Td5">
                                <label id="Label204" style="">角色</label>
                            </td>
                            <td id="Td6">
                                <label id="Label205" style="">ID/注册号</label>
                            </td>
                            <td id="Td7">
                                <label id="Label206" style="">申请人姓名</label>
                            </td>
                            <td id="Td8">
                                <label id="Label207" style="">融资类型</label>
                            </td>
                            <td id="Td9">
                                <label id="Label208" style="">类型</label>
                            </td>
                            <td id="Td20">
                                <label id="Label9" style="">状态日期</label>
                            </td>
                            <td id="Td21">
                                <label id="Label10" style="">期数</label>
                            </td>
                            <td id="Td22">
                                <label id="Label29" style="">生产商</label>
                            </td>
                            <td id="Td23">
                                <label id="Label30" style="">动力参数</label>
                            </td>
                            <td id="Td24">
                                <label id="Label31" style="">车型</label>
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
                            <td id="Td10" class="rowSerialNo"></td>
                            <td id="Td11">
                                <label id="Label32" style="">本金余额</label>
                            </td>
                            <td id="Td12">
                                <label id="Label33" style="">申请号</label>
                            </td>
                            <td id="Td13">
                                <label id="Label34" style="">合同号</label>
                            </td>
                            <td id="Td14">
                                <label id="Label35" style="">状态</label>
                            </td>
                            <td id="Td15">
                                <label id="Label36" style="">申请人状态</label>
                            </td>
                            <td id="Td16">
                                <label id="Label37" style="">角色</label>
                            </td>
                            <td id="Td17">
                                <label id="Label38" style="">ID/注册号</label>
                            </td>
                            <td id="Td18">
                                <label id="Label39" style="">姓名</label>
                            </td>
                            <td id="Td19">
                                <label id="Label40" style="">经销商</label>
                            </td>
                            <td id="Td25">
                                <label id="Label41" style="">合同类型</label>
                            </td>
                            <td id="Td26">
                                <label id="Label42" style="">合同日期</label>
                            </td>
                            <td id="Td27">
                                <label id="Label43" style="">利率</label>
                            </td>
                            <td id="Td28">
                                <label id="Label44" style="">融资额</label>
                            </td>
                            <td id="Td29">
                                <label id="Label45" style="">生产商</label>
                            </td>
                            <td id="Td30">
                                <label id="Label46" style="">动力参数</label>
                            </td>
                            <td id="Td31">
                                <label id="Label377" style="">车型</label>
                            </td>
                            <td id="Td32">
                                <label id="Label47" style="">30天</label>
                            </td>
                            <td id="Td33">
                                <label id="Label379" style="">60天</label>
                            </td>
                            <td id="Td34">
                                <label id="Label381" style="">90天</label>
                            </td>
                            <td id="Td35">
                                <label id="Label395" style="">90天以上</label>
                            </td>
                            <td id="Td36">
                                <label id="Label396" style="">120天以上</label>
                            </td>
                            <td id="Td37">
                                <label id="Label397" style="">期数</label>
                            </td>
                            <td id="Td52">
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
                            <td id="Td53" class="rowSerialNo"></td>
                            <td id="Td54">
                                <label id="Label413" style="">本金余额</label>
                            </td>
                            <td id="Td55">
                                <label id="Label414" style="">申请号</label>
                            </td>
                            <td id="Td56">
                                <label id="Label415" style="">合同号</label>
                            </td>
                            <td id="Td57">
                                <label id="Label416" style="">状态</label>
                            </td>
                            <td id="Td58">
                                <label id="Label417" style="">申请人状态</label>
                            </td>
                            <td id="Td59">
                                <label id="Label419" style="">角色</label>
                            </td>
                            <td id="Td60">
                                <label id="Label421" style="">ID/注册号</label>
                            </td>
                            <td id="Td61">
                                <label id="Label422" style="">姓名</label>
                            </td>
                            <td id="Td62">
                                <label id="Label423" style="">经销商</label>
                            </td>
                            <td id="Td63">
                                <label id="Label424" style="">合同类型</label>
                            </td>
                            <td id="Td64">
                                <label id="Label425" style="">合同日期</label>
                            </td>
                            <td id="Td65">
                                <label id="Label426" style="">利率</label>
                            </td>
                            <td id="Td66">
                                <label id="Label427" style="">融资额</label>
                            </td>
                            <td id="Td67">
                                <label id="Label428" style="">生产商</label>
                            </td>
                            <td id="Td68">
                                <label id="Label429" style="">动力参数</label>
                            </td>
                            <td id="Td69">
                                <label id="Label430" style="">车型</label>
                            </td>
                            <td id="Td70">
                                <label id="Label431" style="">30天</label>
                            </td>
                            <td id="Td71">
                                <label id="Label432" style="">60天</label>
                            </td>
                            <td id="Td72">
                                <label id="Label433" style="">90天</label>
                            </td>
                            <td id="Td73">
                                <label id="Label434" style="">90天以上</label>
                            </td>
                            <td id="Td74">
                                <label id="Label435" style="">120天以上</label>
                            </td>
                            <td id="Td75">
                                <label id="Label436" style="">期数</label>
                            </td>
                            <td id="Td76">
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
     <div id="accessoryDetail" class="modal fade in" aria-hidden="true">
        <div class="modal-header">
            <a class="close" data-dismiss="modal">×</a>
            <h3 style="text-align: center;">附加费详情</h3>
        </div>
        <div class="modal-body">
            <div style="height: auto; min-height: 90px; max-height: 400px; overflow-y: auto;">
        <div class="row tableContent" style="border-right: 0px solid #ccc;border-top: 0px solid #ccc;">
            <div id="control509" class="col-md-10" style="border-left: 0px solid #ccc;width:99%">
                <table id="Control444" data-datafield="CompanyAssetAccessoriesTable" data-type="SheetGridView" style="width:99%;" class="SheetGridView">
                    <tbody>
                        <tr class="header">
                            <td id="Control444_SerialNo" class="rowSerialNo">序号								</td>
                            <td id="Control444_Header4" data-datafield="CompanyAssetAccessoriesTable.additionalName1">
                                <label id="Control444_Label4" data-datafield="CompanyAssetAccessoriesTable.additionalName1" data-type="SheetLabel" style="">选装部件</label>
                            </td>
                            <td id="Control444_Header5" data-datafield="CompanyAssetAccessoriesTable.additionalprice">
                                <label id="Control444_Label5" data-datafield="CompanyAssetAccessoriesTable.additionalprice" data-type="SheetLabel" style="">价格</label>
                            </td>
                            <td class="rowOption">删除								</td>
                        </tr>
                        <tr class="template">
                            <td id="Control444_Option" class="rowOption"></td>
                            <td data-datafield="CompanyAssetAccessoriesTable.additionalName1">
                                <input id="Control444_ctl4" type="text" data-datafield="CompanyAssetAccessoriesTable.additionalName1" data-type="SheetTextBox" style="">
                            </td>
                            <td data-datafield="CompanyAssetAccessoriesTable.additionalprice">
                                <input id="Control444_ctl5" type="text" data-datafield="CompanyAssetAccessoriesTable.additionalprice" data-type="SheetTextBox" style="">
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
                            <td data-datafield="CompanyAssetAccessoriesTable.additionalName1"></td>
                            <td data-datafield="CompanyAssetAccessoriesTable.additionalprice">
                                <label id="Control444_stat5" data-datafield="CompanyAssetAccessoriesTable.additionalprice" data-type="SheetCountLabel" style="">
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
        //敞口
        function getCkje() {
            debugger;
            var IdCardNo = $("#Control3091").val();
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
                    arr['06'] = "待决定";
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
                        tr += "<td>借款公司</td>";
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
                    $("#capckje").html(capckje);
                    console.log(result);
                    getCMSCKJE();
                },
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
                    arr['06'] = "待决定";
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
                            tr += "<td>担保人</td>";
                        } else {
                            tr += "<td>担保公司</td>";
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
                            tr += "<td>担保人</td>";
                        } else {
                            tr += "<td>担保公司</td>";
                        }
                        //tr += "<td>" + sqr + "</td>";
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
                    if (!$("#Application").find("td").length && !$("#cmsApplication").find("td").length) {
                        $("#ckje").find("span").css({ "color": "#000" });
                        $("#ckje").find("a").hide();
                    };
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function getinstence(applicationno) {
            $.ajax({
                //url: "../../../ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandler/getCinstenceid",// 19.6.28 wangxg
                data: { CommandName: "getCinstenceid", applicationno: applicationno },
                type: "POST",
                async: false,
                dataType: "json",
                success: function (result) {
                    if (result.Result == 'nofind') {
                        alert("未查询到此单");
                    }
                    else {
                        if (result.Result.split(";")[1] == "C") {
                            $("#aclick").attr("href", "SCompanyAppTEST.aspx?Mode=View&InstanceId=" + result.Result.split(";")[0]);
                        }
                        if (result.Result.split(";")[1] == "HC") {
                            $("#aclick").attr("href", "HSCompanyApp.aspx?Mode=View&InstanceId=" + result.Result.split(";")[0]);
                        }
                        if (result.Result.split(";")[1] == "R") {
                            $("#aclick").attr("href", "SRetailAppTEST.aspx?Mode=View&InstanceId=" + result.Result.split(";")[0]);
                        }
                        if (result.Result.split(";")[1] == "HR") {
                            $("#aclick").attr("href", "HSRetailApp.aspx?Mode=View&InstanceId=" + result.Result.split(";")[0]);
                        }
                        document.getElementById("aclick").click();
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function capcheck(ts, ck) {
            console.log(ts.dataset.number);
            if (ts.checked) {
                $("#" + ck).html(($("#" + ck).html() * 1) + (ts.dataset.number * 1));
            } else {
                $("#" + ck).html(($("#" + ck).html() * 1) - (ts.dataset.number * 1));
            }
            $("#fullckje").html(($("#capckje").html() * 1) + ($("#cmsckje").html() * 1));
            $("#ctrlCkje").html(($("#cmsckje").html() * 1) + ($("#capckje").html() * 1));
        }
        //外部数据
        function rsfkClick() {
            var rsfkurl = '<%=ConfigurationManager.AppSettings["rsfkurl"] + string.Empty%>';
            var InstanceId = '<%=this.ActionContext.InstanceId%>';
            var url = "../view/CrsfkresultTEST.html?&rsfkurl=" + rsfkurl + "&InstanceId=" + InstanceId;
            //var url = "../view/CrsfkresultTEST.html?&rsfkurl=http://192.168.16.102:8088/request/result&InstanceId=" + InstanceId;
            $("#aclick").attr("href", url);
            document.getElementById("aclick").click();
            return false;
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
                url: "/Portal/RSHandler/getRSResult",// 19.6.28 wangxg
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
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
            return false;
        }
        //获取nciic
        function nciicClick() {
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            var nciicurl = '<%=ConfigurationManager.AppSettings["nciicurl"] + string.Empty%>';
            var url = "../view/CNCIIC.html?&nciicurl=" + nciicurl + "&instanceid=" + instanceid;
            //var url = "../view/CNCIIC.html?&nciicurl=http://192.168.16.102:8088/response/searchnciic&instanceid=" + instanceid;
            $("#aclick").attr("href", url);
            document.getElementById("aclick").click();
            return false;
        }
        //融数
        //人工查询
        function rsmanchk() {
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx?Math=" + Math.random(),
                url: "/Portal/RSHandler/postRongshu?Math=" + Math.random(),// 19.6.28 wangxg
                data:
                {
                    CommandName: "postRongshu",
                    instanceid: instanceid,
                    SchemaCode: "CompanyApp",
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
                url: "/Portal/RSHandler/getRSResult",// 19.6.28 wangxg
                data:
                {
                    CommandName: "getRSResult",
                    param: "{\"reqID\": \"" + instanceid + "\" }",
                    address: rsfkurl 
                    //address: "http://192.168.16.102:8088/request/result"
                },
                type: "post",
                async: true,
                dataType: "json",
                success: function (result) {
                    debugger;
                    if (result.code == "00") {
                        $("#divBascSQRInfo").find(">div:eq(0)").find("a").show();
                        $("#searching").html("");
                        printtext(result);
                        getNciic();
                    } else {
                        setTimeout("timegets()", 10000);
                    }
                },
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
            var rsfkurl = '<%=ConfigurationManager.AppSettings["rsfkurl"] + string.Empty%>';
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx",
                url: "/Portal/RSHandler/getRSResult",// 19.6.28 wangxg
                data:
                {
                    CommandName: "getRSResult",
                    param: "{\"reqID\": \"" + instanceid + "\" }",
                    address: rsfkurl
                    //address: "http://192.168.16.102:8088/request/result"
                },
                type: "post",
                dataType: "json",
                success: function (result) {
                    debugger;
                    printtext(result);
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function getNciic() {
            var instanceid = '<%=this.ActionContext.InstanceId%>';
            var nciicurl = '<%=ConfigurationManager.AppSettings["nciicurl"] + string.Empty%>';
            $.ajax({
                //url: "../../../ajax/RSHandler.ashx",
                url: "/Portal/RSHandler/getRSResult",// 19.6.28 wangxg
                data:
                {
                    CommandName: "getRSResult",
                    param: "{\"reqID\": \"" + instanceid + "\" }",
                    address: nciicurl
                    //address: "http://192.168.16.102:8088/response/searchnciic"
                },
                type: "post",
                async: true,
                dataType: "json",
                success: function (result) {
                    if (result.code == "00") {
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
                if (result.ds.dbr && result.ds.dbr.ds1__product__result == "不一致") {
                    var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                    $("#control301").append(span);
                } else {
                    var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                    $("#control301").append(span);
                }
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
        //隐藏附加上传空白行
        function hideFIrow() {
            $("#divattachment .row").each(function () {
                if ($(this).find(">.col-md-2>span").css("display") == "none") {
                    $(this).hide();
                }
            })
        }
        //标红
        function isred() {
            if ($("#divOriginateOUName").find('label').html() == "外网经销商") {
                $("#divOriginateOUName").find('label').css({ "color": "red" });
            }
            if ($("#control63").find("label").find("span").html() == "是") {
                $("#control63").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control258").find("label").find("span").html() == "是") {
                $("#control258").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control393").find("label").find("span").html() == "是") {
                $("#control393").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control223").find("label").find("div").html() == "非本地户口") {
                $("#control223").find("label").find("div").css({ "color": "red" });
            }
            if ($("#control331").find("label").find("span").html() == "否") {
                $("#control331").find("label").find("div").css({ "color": "red" });
            }
            if ($("#control340").find("label").find("span").html() * 1 >= $("#control309").find("label").find("span").html() * 1) {
                $("#control340").find("label").find("span").css({ "color": "red" });
            }
            if ($("#control342").find("label").find("span").html() * 1 >= $("#control309").find("label").find("span").html() * 1) {
                $("#control342").find("label").find("span").css({ "color": "red" });
            }
            if ($("#Control3111").val() == $("#Control250").val()) {
                var span = "<label><span style=\"padding-left:15px!important;\">一致</span></label>";
                $("#control261").append(span);
            } else {
                var span = "<label><span style=\"color:red;padding-left:15px!important;\">不一致</span></label>";
                $("#control261").append(span);
            }
        }
        //审核信息顶部固定
        function setfixtop() {
            $("#fktop").css({ "position": "fixed", "top": $("#main-navbar").outerHeight(), "left": $("#main-navbar").offset().left, "width": $("#content-wrapper").outerWidth() });
            $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
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
        //隐藏二级菜单
        function hidemsg() {
            //担保人不存在隐藏
            if ($("#Control211").val() == "") {
                $(".bannerTitle")[3].click();
                $($(".bannerTitle")[3]).hide();
                $("#aclick").parent().find("a:eq(0)").hide();
                $(".fengkong").find("a").hide();
                $("#fjxq").show();
                
            }else  if ($("#Control380").val() == "overtime") { //隐藏融数
                    $(".fengkong").find("a").hide();
                    $("#rsmanchk").show();
                    $("#fjxq").show();
                    $("#rsmanchk").on('click', rsmanchk());
                } else {
                    getRS();
                    getNciic();
                } 
            //担保公司不存在隐藏
            if ($("#Control3342").val() == "") {
                $(".bannerTitle")[4].click();
                $($(".bannerTitle")[4]).hide();
            }
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
        var viewer;
        // 页面加载完成事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            getCkje();
            isred();
            setfixtop();
            hideFIrow();
            getmsg();
            hidemsg();
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

        $.MvcSheet.SaveAction.OnActionDone = function () {
            // this.Action  // 获取当前按钮名称
            //alert($.MvcSheetUI.SheetInfo.InstanceId);
            //$.MvcSheetUI.SetControlValue("instanceid", $.MvcSheetUI.SheetInfo.InstanceId);
        }
        // 表单验证接口
        $.MvcSheet.Validate = function () {  
            //信审初审
            if (this.Action == "Submit" && $.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
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

            return true;
        }
    </script>
</asp:Content>
