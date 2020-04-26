<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XSSH.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.XSSH" EnableEventValidation="false" MasterPageFile="~/MvcSheetTEST.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link href="css/common.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" rel="stylesheet" />
    <link href="css/XSSH.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" rel="stylesheet" />
    <link href="/Portal/WFRes/css/MvcSheetTest.css" rel="stylesheet" />
    <link type="text/css" href="/Portal/jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <link type="text/css" href="/Portal/css/build.css" rel="stylesheet" />
    <link type="text/css" href="/Portal/css/font-awesome.min.css" rel="stylesheet" />
    

    <script src="/Portal/jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Portal/WFRes/layer/layer.js?20180712"></script>
    <script type="text/javascript">
        var rsfkurl = '<%=ConfigurationManager.AppSettings["rsfkurl"] + string.Empty%>';
        var pbocurl = '<%=ConfigurationManager.AppSettings["pbocurl"] + string.Empty%>';
        var nciicurl = '<%=ConfigurationManager.AppSettings["nciicurl"] + string.Empty%>';
    </script>
    <script type="text/javascript" src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <script type="text/javascript" src="js/XSSH.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <script type="text/javascript" src="../../../../Custom/js/acl.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
    <div class="panel-body sheetContainer" style="padding: 0;">
        <div id="fktop">
            <div class="row fengkong">
                <div id="wbsjy" style="width: 50%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a href="javascript:void(0);" onclick="rsfkClick();">风控评估模型评估结果</a></label>
                    <label id="lb-fkpgjg"><span id="fkpgjg" style="padding-left: 5px; padding-right: 5px;"></span></label>
                    <label id="lb-yusuan"><span style="padding-left: 5px;">客户还款测算:</span></label>
                    <label id="lb-srfzc"><span id="srfzc" style="padding-left: 5px;"></span></label>
                    <label style="min-width: 30%;" id="xypf">
                        <span>信用评分:</span><%--新的风控隐藏--%>
                        <span id="grxypf" style="padding-left: 5px;"></span>
                    </label>
                    <label id="dzdatamodel">
                        <span>东正大数据模型:</span>
                        <input id="dzdsjmx" type="text" data-datafield="sqrdsjmx" data-type="SheetTextBox" style="">
                    </label>
                    <label class="hidden" id="lb-fkshowresult">
                        <input id="Control222" type="text" data-datafield="FK_SHOWRESULT" data-type="SheetTextBox" style="">
                    </label>
                    <label id="lb-grade" style="color:red"><span style="padding-left: 5px;">客户评级：</span><span id="sp_grade" style=""></span></label>
                </div>
                <div style="width: 5%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a href="javascript:void(0);" onclick="nciicClick();">NCIIC</a></label>
                </div>
                <div style="width: 25%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;" id="aclickh">
                    <label>PBOC</label>
                </div>
                <div class="hidden" style="width: 25%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;" id="fk_aclickh">
                    <label id="fk_pboc_label">PBOC</label>
                </div>

                <div id="rgcx" style="width: 10%; line-height: 20px; padding: 5px 0px 5px 0px; float: left; position: relative;">
                    <a class="btn" id="rsmanchk" onclick="rsmanchk()">人工查询</a> <%--新的风控隐藏--%>
                    <span id="searching" style="position: absolute; white-space: nowrap;"></span>
                </div>
                <div style="width: 15%; line-height: 20px; padding: 10px 5px 5px 5px; float: left;">
                    <label><a href="javascript:void(0);" onclick="getDownLoadURL()">查看附件资料</a></label>
                    <label id="lb_datatracklog" class="hidden"><a href="javascript:void(0);" onclick="getDataTrackLog()">查看FI数据日志</a></label>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv_xs('div2',this)">
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
                        <div id="control467" class="leftbox1" style="width: 8%; position: relative; left: 34px; top: 6px;">
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
                        <div data-datafield="csshzt" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultselected="false" data-defaultitems="核准;拒绝;有条件核准;驳回;取消"></div>
                        <div id="csjjxz">
                            <div id="control493" class="rightbox" style="width: 50%; padding: 5px 0!important;">
                                <select data-datafield="xsjjlyzx" data-type="SheetDropDownList" id="ctl974208" class="" style=""
                                    data-schemacode="shyj1" data-querycode="shyj" data-displayemptyitem="true" data-filter="csshzt:TYPE2,个人零售:TYPE1,0:PARENTNAME"
                                    data-datavaluefield="REJECTNAME" data-datatextfield="REJECTNAME" data-onchange="cstjChange(this)">
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
                            <div data-datafield="zsshzt" data-type="SheetRadioButtonList" id="ctl271485" class="" style="" data-defaultitems="核准;拒绝;驳回;取消"></div>
                            <div id="Control362" data-datafield="ZSYJj" data-type="SheetComment" style="">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row tableContent" id="row-facesign">
                    <div id="title529" class="leftbox1">
                        <span id="Label582" data-type="SheetLabel" style="">是否需要电子面签</span>
                    </div>
                    <div id="control525" class="rightbox1">
                        <select id="Control58" data-datafield="NeedFaceSign" data-type="SheetDropDownList" style="" data-queryable="false">
                            <option value="">请选择</option>
                            <option value="0">不需要</option>
                            <option value="1">需要</option>
                        </select>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label data-en_us="Basic information">经销商基本信息</label>
            <div style="float: right; font-weight: 300; font-size: 14px; padding-right: 5%; cursor: pointer;" onclick="showliuyan()">
                <img src="../../../../img/Images/liuyan.png" style="position: relative; top: -4px;" />
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
                <div class="row" style="display: none;">
                    <div id="title421" class="col-md-2">
                        <span id="Label380" data-type="SheetLabel" style="">风控审核结果</span>
                    </div>
                    <div id="control421" class="leftbox">
                        <input id="Control380" data-datafield="fkResult" data-type="SheetTextBox" style="">
                        <a href="#" id="aclick" target="_blank" style="display: none;"></a>
                    </div>
                </div>
                <div class="row" hidden>
                    <div id="title422" class="col-md-2">
                        <span id="Label381" data-type="SheetLabel" style="">报告Id</span>
                    </div>
                    <div id="control423" class="leftbox">
                        <input id="Control589" data-datafield="FK_RECORDID" data-type="SheetTextBox" style="">
                        <a href="#" id="aclick2" target="_blank" style="display: none;"></a>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title1" class="leftbox">
                            <span id="Label11" data-type="SheetLabel" data-datafield="APPLICATION_NUMBER" style="">贷款申请号码</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control1" class="rightbox">
                            <input id="applicationNo" type="text" data-datafield="APPLICATION_NUMBER" data-type="SheetTextBox" style="">
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
                <div class="row">
                    <div class="col-md-4">
                        <div id="Div1" class="leftbox">
                            <span id="Label448" data-type="SheetLabel" data-datafield="dycs" style="">抵押城市</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="Div3" class="rightbox" style="color: red;">
                            <input id="Control451" type="text" data-datafield="dydz" data-type="SheetTextBox" style="">
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
                            <span id="Label12" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_CODE" style="">申请类型</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control2" class="rightbox3">
                            <select id="Control12" data-datafield="APPLICATION_TYPE_CODE" data-type="SheetDropDownList" style="" data-masterdatacategory="申请类型" data-queryable="false"></select>
                        </div>
                    </div>
                </div>
                <div class="row hidden">
                    <div class="col-md-4">
                        <div class="leftbox">
                            bp_id
                        </div>
                        <div class="centerline"></div>
                        <div class="rightbox">
                            <input type="text" id="hidden_001" data-datafield="bp_id" data-type="SheetTextBox" style="" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox">
                            USER_NAME
                        </div>
                        <div class="centerline"></div>
                        <div class="rightbox">
                            <input type="text" id="hidden_002" data-datafield="USER_NAME" data-type="SheetTextBox" style="" data-defaultvalue="{Originator.LoginName}" />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="leftbox">
                            PLAN_ID
                        </div>
                        <div class="centerline"></div>
                        <div class="rightbox">
                            <input type="text" id="hidden_003" data-datafield="PLAN_ID" data-type="SheetTextBox" style="" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title4" class="leftbox2">
                            <span id="Label14" data-type="SheetLabel" data-datafield="BP_COMPANY_ID" style="">申请贷款公司</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control4" class="rightbox2">
                            <select id="Control14" data-datafield="BP_COMPANY_ID" data-type="SheetDropDownList" data-masterdatacategory="申请贷款公司" style="" data-queryable="false"></select>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title5" class="leftbox">
                            <span id="Label15" data-type="SheetLabel" data-datafield="BP_DEALER_ID" style="">金融顾问</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control5" class="rightbox">
                            <select id="Control15" data-datafield="BP_DEALER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                                data-schemacode="M_Financial_Adviser" data-querycode="006" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title6" class="leftbox">
                            <span id="Label16" data-type="SheetLabel" data-datafield="VEHICLE_TYPE_CDE" style="">车辆类型</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control6" class="rightbox">
                            <select id="Control16" data-datafield="VEHICLE_TYPE_CDE" data-type="SheetDropDownList" style=""
                                data-schemacode="M_CarType" data-querycode="001" data-datavaluefield="VEHICLE_TYPE_CDE" data-datatextfield="VEHICLE_TYPE_DSC" data-selectedvalue="00001">
                            </select>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title7" class="leftbox2">
                            <span id="Label17" data-type="SheetLabel" data-datafield="STATE_CDE" style="">经销商省</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control7" class="rightbox2">
                            <select id="Control17" data-datafield="STATE_CDE" data-type="SheetDropDownList" style="" data-queryable="false"
                                data-schemacode="M_Province" data-querycode="004" data-datavaluefield="STATE_CDE" data-datatextfield="STATE_NME" data-filter="bp_id:bp_id">
                            </select>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title8" class="leftbox3">
                            <span id="Label18" data-type="SheetLabel" data-datafield="CITY_CDE" style="">经销商城市</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control8" class="rightbox3">
                            <select id="Control18" data-datafield="CITY_CDE" data-type="SheetDropDownList" style="" data-queryable="false"
                                data-schemacode="M_City" data-querycode="005" data-datavaluefield="CITY_CDE" data-datatextfield="CITY_NME" data-filter="bp_id:bp_id">
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title9" class="leftbox">
                            <span id="Label19" data-type="SheetLabel" data-datafield="BP_SALESPERSON_ID" style="">销售人员</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control9" class="rightbox">
                            <select id="Control19" data-datafield="BP_SALESPERSON_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                                data-schemacode="M_sale_man" data-querycode="002" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                            </select>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title10" class="leftbox2">
                            <span id="Label20" data-type="SheetLabel" data-datafield="BP_SHOWROOM_ID" style="">展厅</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control10" class="rightbox2">
                            <select id="Control20" data-datafield="BP_SHOWROOM_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                                data-schemacode="M_exhibition_hall" data-querycode="007" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                            </select>

                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title11" class="leftbox3">
                            <span id="Label21" data-type="SheetLabel" style="">制造商</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control11" class="rightbox3">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title12" class="leftbox">
                            <span id="Label22" data-type="SheetLabel" data-datafield="CONTACT_PERSON" style="">联系方式</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control12" class="rightbox">
                            <input id="Control22" type="text" data-datafield="CONTACT_PERSON" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title13" class="leftbox2">
                            <span id="Label23" data-type="SheetLabel" data-datafield="REFERENCE_NBR" style="">申请参考号</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control13" class="rightbox2">
                            <input id="Control23" type="text" data-datafield="REFERENCE_NBR" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title14" class="leftbox3">
                            <span id="Label24" data-type="SheetLabel" data-datafield="BUSINESS_PARTNER_ID" style="">经销商</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control14" class="rightbox3">
                            <select id="Control24" data-datafield="BUSINESS_PARTNER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                                data-schemacode="M_business_partner_id" data-querycode="003" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <div id="title15" class="leftbox">
                            <span id="Label25" data-type="SheetLabel" data-datafield="USER_NAME" style="">账户名</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control15" class="rightbox">
                            <input id="Control25" type="text" data-datafield="USER_NAME" data-type="SheetTextBox" style="">
                        </div>
                    </div>
                </div>
            </div>
            <div data-isretract="true" class="isshowdetail">
                <a href="javascript:void(0);" onclick="hideInfo('divBaseInfo1',this)">收起 &and;</a>
            </div>
        </div>
        <div class="nav-icon bannerTitle">
            <label id="Label5" data-en_us="Sheet information">人员信息</label>
        </div>
        <div class="divContent">
            <ul id="tab_borrower" class="nav nav-tabs borrower">
            </ul>
            <div id="div_borrower" class="tab-content" style="padding: 0px">
                <div class="template I">
                    <div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox ">
                                    姓名（中文）
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox ">
                                    <span data-dtf="APPLICANT_DETAIL.FIRST_THI_NME"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    电话
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_PHONE_FAX.PHONE_NUMBER"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox">
                                    关系
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    证件号码
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.ID_CARD_NBR"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2 ">
                                    户口所在地
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2 ">
                                    <span data-dtf="APPLICANT_DETAIL.HUKOU_CDE" data-style="value=='非本地户口'?'color:red'?''"></span>
                                </div>
                            </div>
                            <div class="col-md-4 hidden">
                                <div class="leftbox3">
                                    Spouse
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.SPOUSE_IND" data-options="false:否;true:是"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox ">
                                    出生日期
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox ">
                                    <span data-dtf="APPLICANT_DETAIL.DATE_OF_BIRTH" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2 ">
                                    教育程度
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2 ">
                                    <span data-dtf="APPLICANT_DETAIL.EDUCATION_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    第三方黑名单记录
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3" id="hmdjl">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox ">
                                    驾照状态
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox ">
                                    <span data-dtf="APPLICANT_DETAIL.DRIVING_LICENSE_CODE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2 ">
                                    婚姻状况
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2 ">
                                    <span data-dtf="APPLICANT_DETAIL.MARITAL_STATUS_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    第三方外部信用记录
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3" id="wbxyjl">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    申请人地址
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="ADDRESS.UNIT_NO"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3 ">
                                    籍贯
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3 ">
                                    <span data-dtf="ADDRESS.NATIVE_DISTRICT"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1 ">
                                    户籍地址
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="ADDRESS.ADDRESS_ID"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3 ">
                                    户籍省份
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3 ">
                                    <span data-dtf="ADDRESS.STATE_CDE4"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    公司名称
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="EMPLOYER.NAME_2"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    抵押人
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.LIENEE" data-options="true:是;false:否" data-style="value=='是'?'color:red'?''"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    企业性质
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="EMPLOYER.BUSINESS_NATURE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    公司电话
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="EMPLOYER.PHONE"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divtoggle">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox ">
                                    月收入
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox ">
                                    <span data-dtf="APPLICANT_DETAIL.ACTUAL_SALARY"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    单位城市
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="EMPLOYER.CITY_CDE6"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    性别
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.SEX"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    国籍
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.RACE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    证件发行日
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.ID_CARDISSUE_DTE" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    证件到期日
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.ID_CARDEXPIRY_DTE" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    驾照号码
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.LICENSE_NUMBER"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    驾照到期日
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.LICENSE_EXPIRY_DATE" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    名（英文）
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.FIRST_NAME"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    中间名字
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.MIDDLE_NAME"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    姓（英文）
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.LAST_NAME"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    曾用名
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.FORMER_NAME"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    民族
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.NATION_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    签发机关
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.ISSUING_AUTHORITY"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    供养人数
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.NUMBER_OF_DEPENDENT"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    房产所有人
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.HOUSE_OWNER_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    家庭人数
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.NO_OF_FAMILY_MEMBERS"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    是否是子女
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.CHILDREN_FLAG" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    雇员类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="EMPLOYER.EMPLOYER_TYPE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    邮箱地址
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.EMAIL_ADDRESS"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    行业类型
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.INDUSTRY_TYPE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    行业子类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.INDUSTRY_SUBTYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    职业类型
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.OCCUPATION_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    职业子类型
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.SUB_OCCUPATION_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    职位
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.DESIGNATION_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    工作组
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_DETAIL.JOB_GROUP_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    估计收入
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.SALARY_RANGE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    同意
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.PRIVACY_ACT" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    贵宾
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.VIP_IND" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    工作人员
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.STAFF_IND" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    黑名单无记录
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_DETAIL.BLACKLIST_NORECORD_IND" data-options="true:是;false:否" data-style="value=='是'?'color:red'?''"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    外部信用记录
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox ">
                                    <span data-dtf="APPLICANT_DETAIL.BLACKLIST_NORECORD_IND" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    职位
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="EMPLOYER.DESIGNATION_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    单位省份
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="EMPLOYER.STATE_CDE2"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    传真
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.FAX"></span>
                                </div>
                            </div>
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    公司地址
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.ADDRESS_ONE_2"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    雇主类型
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.EMPLOYER_TYPE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    雇员人数
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.NUMBER_OF_EMPLOYEES"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    工作描述
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="EMPLOYER.JOB_DESCRIPTION"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    工作年限（月）
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.TIME_IN_MONTH_2"></span>
                                </div>
                            </div>
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    公司评论
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.EMPLOYER_COMMENT"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    默认邮寄地址
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.DEFAULT_ADDRESS" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    户籍地址
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.HUKOU_ADDRESS" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    国家
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="ADDRESS.COUNTRY_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    出生地省市县（区）
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.BIRTHPLEASE_PROVINCE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    户籍城市
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.CITY_CDE4" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    公司邮编
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="EMPLOYER.POST_CODE_3"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    邮编
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.POST_CODE_2"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    地址类型
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.ADDRESS_TYPE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    地址状态
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="ADDRESS.ADDRESS_STATUS"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    房产类型
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.PROPERTY_TYPE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    住宅类型
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.RESIDENCE_TYPE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    开始居住日期
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="ADDRESS.LIVING_FROM_DTE" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    房屋价值（万元)
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.HOME_VALUE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    居住年限
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.TIME_IN_YEAR"></span>年
                                <span data-dtf="ADDRESS.TIME_IN_MONTH"></span>月
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    国家代码
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_PHONE_FAX.COUNTRY_CODE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    地区代码
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_PHONE_FAX.AREA_CODE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    分机
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_PHONE_FAX.EXTENTION_NBR"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    电话类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_PHONE_FAX.PHONE_TYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    联系人名称
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="PERSONNAL_REFERENCE.NAME10"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    与联系人关系
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="PERSONNAL_REFERENCE.RELATIONSHIP_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    联系人地址
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="PERSONNAL_REFERENCE.ADDRESS_ONE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    联系人省份
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="PERSONNAL_REFERENCE.STATE_CDE10"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    联系人城市
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="PERSONNAL_REFERENCE.CITY_CDE10"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    联系人邮编
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="PERSONNAL_REFERENCE.POST_CODE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    联系人电话
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="PERSONNAL_REFERENCE.PHONE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    联系人手机
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox">
                                    <span data-dtf="PERSONNAL_REFERENCE.MOBILE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    联系人户口所在地
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="PERSONNAL_REFERENCE.HOKOU_TYPE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    工作年限（年）
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="EMPLOYER.TIME_IN_YEAR_2"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    证件类型
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_DETAIL.ID_CARD_TYP"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="td_report_link" class="hidden" style="position: absolute; right: 0; top: 28px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                        <a>同盾报告</a>
                    </div>
                    <div id="bankcard_report_link" class="hidden" style="position: absolute; right: 0; top: 58px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                        <a>银行卡信息报告</a>
                    </div>
                    <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                        <a onclick="hideInfo_New('divtoggle',this)">收起 &and;</a>
                    </div>
                </div>
                <div class="template C">
                    <div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    公司名称
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="COMPANY_DETAIL.COMPANY_THI_NME"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    组织机构代码
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.ORGANIZATION_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    企业类型
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="COMPANY_DETAIL.BUSINESS_TYPE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    注册号
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.COMPANY_STS"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    地址
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="ADDRESS.UNIT_NO"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    注册资本金额
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.CAPITAL_REGISTRATION_AMT"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="leftbox1">
                                    注册地址
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div class="rightbox1">
                                    <span data-dtf="ADDRESS.REGISTERED_ADDRESS"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    行业类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.INDUSTRY_TYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    法人姓名
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.REP_NAME"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    电话
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_PHONE_FAX.PHONE_NUMBER"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    抵押人
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.LIENEE" data-options="true:是;false:否" data-style="value=='是'?'color:red'?''"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    成立自
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.ESTABLISHED_SINCE" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    房产类型
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="ADDRESS.PROPERTY_TYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divtoggle">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    公司年限
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.BUSINESS_HISTORY"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    母公司
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="COMPANY_DETAIL.PARENT_COMPANY"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    子公司
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.SUBSIDIARY_COMPANY"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    法人身份证件号码
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.REP_ID_CARD_NO"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    法人职务
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="COMPANY_DETAIL.REPRESENTATIVE_DESIGNATION"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    贷款卡号
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.LENDING_CODE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    贷款卡密码
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.LOAN_CRD_PWD"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    公司名称(英文)
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="COMPANY_DETAIL.NAME3"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    税号
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.EMPLOYEE_NUMBER"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    信托机构名称
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.FUTURE_DIRECTION"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    收入
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="COMPANY_DETAIL.FLEET_SIZE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    净资产额
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.NET_WORTH_AMT"></span>
                                </div>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    年份
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.ESTABLISHED_SINCE_YY"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    邮箱地址
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="COMPANY_DETAIL.EMAIL_ADDRESS"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    外部信用记录
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.BLACKLIST_IND" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    黑名单无记录
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="COMPANY_DETAIL.BLACKLIST_NORECORD_IND" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    默认邮寄地址
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="ADDRESS.DEFAULT_ADDRESS" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    办公地址
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="ADDRESS.HUKOU_ADDRESS" data-options="true:是;false:否"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    国家
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.COUNTRY_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    邮编
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="ADDRESS.POST_CODE_2"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    地址类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="ADDRESS.ADDRESS_TYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    地址状态
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.ADDRESS_STATUS"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    住宅类型
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="ADDRESS.RESIDENCE_TYPE_CDE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    开始居住日期
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="ADDRESS.LIVING_FROM_DTE" data-fmt="yyyy-MM-dd"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    房屋价值（万元)
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.HOME_VALUE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    居住年限
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="ADDRESS.TIME_IN_YEAR"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    国家代码
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_PHONE_FAX.COUNTRY_CODE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    地区代码
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="APPLICANT_PHONE_FAX.AREA_CODE"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    分机
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="APPLICANT_PHONE_FAX.EXTENTION_NBR"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    电话类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="APPLICANT_PHONE_FAX.PHONE_TYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    省份
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <span data-dtf="ADDRESS.STATE_CDE4"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox2">
                                    城市
                                </div>
                                <div class="centerline2"></div>
                                <div class="rightbox2">
                                    <span data-dtf="ADDRESS.CITY_CDE4"></span>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="leftbox3">
                                    行业子类型
                                </div>
                                <div class="centerline3"></div>
                                <div class="rightbox3">
                                    <span data-dtf="COMPANY_DETAIL.INDUSTRY_SUBTYPE_CDE"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="nav-icon bannerTitle">
        <label data-en_us="Basic information">添加电话</label>
    </div>
    <div class="divContent" id="">
        <div style="margin-bottom: 10px; margin-top: 10px;">
            <div style="float:left">
                <input type="button" class="btn btn-primary" id="addPhoneExtension"  value="添加电话" />
            </div>
            <div style="float:right">
                <input type="button" id="EmptyPhone" class="btn btn-info"  value="取消" />
                <input type="button" id="savePhone" class="btn btn-primary"  value="保存" />
            </div>
            <div class="row tableContent" style="clear:both; border: none; padding-bottom: 30px;" id="">
                <table class="SheetGridView" style="margin-top:10px;text-align: center; width: 100%;">
                    <tr class="header">
                        <td class="rowSerialNo">序号</td>
                        <td class="">被叫人姓名</td>
                        <td class="">被叫人角色类型</td>
                        <td class="">被叫人电话</td>
                        <td class="">电话类型</td>
                        <td class="">删除</td>
                    </tr>
                    <tbody id="tabAddPhone">
                        <tr>
                            <td style="display: none">
                                <input type="hidden" id="OBJECTID" name="OBJECTID" /></td>
                            <td>
                                <%--<input type="text" />--%>
                            </td>
                            <td>
                                <input type="text" id="CALLEDPARTYNAME" name="CALLEDPARTYNAME" /></td>
                            <td>
                                <%--<input type="text" id="CALLEDPARTYTYPE" name="CALLEDPARTYTYPE" /></td>--%>
                                <select id="CALLEDPARTYTYPE" name="CALLEDPARTYTYPE">
                                    <option value="主借人">主借人</option>
                                    <option value="共借人">共借人</option>
                                    <option value="担保人">担保人</option>
                                    <option value="紧急联系人（亲属）">紧急联系人（亲属）</option>
                                    <option value="紧急联系人（其他）">紧急联系人（其他）</option>
                                    <option value="单位">单位</option>
                                    <option value="住宅">住宅</option>
                                    <option value="金融顾问">金融顾问</option>
                                </select>
                            <td>
                                <input type="text" id="CALLEDPARTYNUMBER" name="CALLEDPARTYNUMBER" /></td>
                            <td>
                                <select id="CALLEDPARTYNUMBERTYPE" name="CALLEDPARTYNUMBERTYPE">
                                    <option value="座机">座机</option>
                                    <option value="手机 Mobile Phone">手机 Mobile Phone</option>
                                </select></td>
                            <td></td>
                        </tr>
                    </tbody>
                    <tbody id="tabPhones">
                        
                    </tbody>
                </table>
            </div>
           
        </div>
    </div>

    <!-- 模态框（Modal） -->
    <div class="modal fade" id="myModal1" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        &times;
                    </button>
                    <h4 class="modal-title" >拨打电话</h4>
                </div>
                <div class="modal-body" style="background-color:white">
                    <input type="text" hidden id="dialCalledName" placeholder="姓名" />
                    <input type="text" hidden id="dialCalledNumber" placeholder="号码" />
                    <input type="text" hidden id="dialCalledType" placeholder="角色" />
                    <input type="text" hidden id="dialMobileType" placeholder="电话类型" />
                    <input type="text" hidden id="dialIdType" placeholder="证件类型" />
                    <input type="text" hidden id="dialIdNumber" placeholder="证件号码" />
                    <input type="text" hidden id="dialContractNo" placeholder="合同号" />

                    <div class="radio radio-danger" style="margin-top:15px;text-align:center">
                        <input type="radio" name="phoneposition" id="radio3" value="hostplace">
                        <label for="radio3">
                            本地号码
                        </label>
                    </div>
                    <div class="radio radio-danger" style="margin-top:15px;text-align:center;margin-bottom:15px">
                        <input type="radio" name="phoneposition" id="radio4" value="otherplace">
                        <label for="radio4">
                            外地号码
                        </label>
                    </div>
                </div>
                <div class="modal-footer" style="text-align:center">
                    <input type="button" class="btn btn-primary" id="Dial" value="拨打" onclick="dialOut()" style="width:150px;height:30px; border-radius: 9px;" />
                </div>
            </div>
        </div>
    </div>

    <!-- 模态框（Modal） -->
    <div class="modal fade" id="myModal2" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog  modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close hidden" data-dismiss="modal" aria-hidden="true">
                        &times;
                    </button>
                    <h4 class="modal-title">拨打电话</h4>
                </div>
                <div class="modal-body">
                    <div style="margin-top:15px;text-align:center;margin-bottom:15px">
                        <img id="photo" class="" src="../V1/img/guaduan.png" width="100" height="100">
                    </div>
                </div>
                <div class="modal-footer" style="text-align:center">
                   <input type="button" class="btn btn-primary" id="HangUp" value="挂断" onclick="handUp()" style="width:150px;height:30px; border-radius: 9px;" />
                </div>
            </div>
            </div>
        </div>

    <div class="nav-icon bannerTitle">
        <label data-en_us="Basic information">录音列表</label>
    </div>
    <div class="divContent" id="">
        <div style="margin-bottom: 10px; margin-top: 10px;">
            <input type="button" id="" value="录音列表" style="height: 40px; width: 13%; background-color: cornflowerblue; color: white;" />
            <div style="margin-bottom: 10px; margin-top: 10px;">
                <input type="text" id="calledName"  placeholder="请输入被叫人姓名" style="width: 15%" />
                <%--<input type="text" id="calledType"  placeholder="请输入角色类型" style="width: 15%;margin-left:15px" />--%>
                <select id="calledType" style="width: 15%;margin-left:15px" >
                    <option value="">请选择角色类型</option>
                    <option value="主借人">主借人</option>
                    <option value="共借人">共借人</option>
                    <option value="担保人">担保人</option>
                    <option value="紧急联系人（亲属）">紧急联系人（亲属）</option>
                    <option value="紧急联系人（其他）">紧急联系人（其他）</option>
                    <option value="单位">单位</option>
                    <option value="住宅">住宅</option>
                    <option value="金融顾问">金融顾问</option>
                </select>

                <input type="text" id="calledNumber" placeholder="请输入被叫人电话" style="width: 15%;margin-left:15px" />
                <input type="button" id="RecordingConditions" value="搜索" class="btn btn-primary"  style="" />
            </div>
            <table class="SheetGridView" style="text-align: center; width: 100%;">
                
                <tr class="header">
                    <th class="rowSerialNo" style="width:5%">序号</th>
                    <th class="" style="">被叫人姓名</th>
                    <th class="" style="">被叫人角色类型</th>
                    <th class="" style="">被叫人电话</th>
                    <th class="" style="">电话类型</th>
                    <th class="" style="">录音生成时间</th>
                    <th class="" style="">录音播放</th>
                </tr>
                <tbody id="RecordTab">
                    <%--<tr>
                        <td>
                            <input type="text" id="" /></td>
                        <td>
                            <input type="text" id="" /></td>
                        <td>
                            <input type="text" id="" /></td>
                        <td>
                            <input type="text" id="" /></td>
                        <td>
                            <input type="text" id="" /></td>
                        <td>
                            <input type="text" id="" /></td>
                        <td>
                            <input type="text" id="" /></td>

                    </tr>--%>
                </tbody>
            </table>
        </div>
    </div>


    <div class="nav-icon bannerTitle">
        <label id="Label3" data-en_us="Basic information">资产信息</label>
    </div>
    <div class="divContent" id="divZC">
        <div>
            <div id="ctl524044" data-datafield="VEHICLE_DETAIL" data-type="SheetDetail" class="" style=""
                data-defaultrowcount="1" data-displayadd="false">

                <ul id="myTab_ctl524044" class="nav nav-tabs hidden">
                </ul>
                <div id="myTabContent_ctl524044" class="tab-content" style="padding: 0px">
                    <div class="template">
                        <div class="row hidden">
                            <div class="col-md-4">
                                <div id="ctl524044_Title2" class="col-md-4">
                                    <span id="ctl524044_label2" style="">IDENTIFICATION_CODE</span>
                                </div>
                                <div id="ctl524044_Data2" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.IDENTIFICATION_CODE7" data-type="SheetTextBox" id="ctl524044_control2" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524045_Title3" class="col-md-4">
                                    <span id="ctl524045_label3" style="">制造商Code</span>
                                </div>
                                <div id="ctl524045_Data3" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MAKE_CDE" data-type="SheetTextBox" id="ctl524045_control3" style="" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524045_Title75" class="col-md-4">
                                    <span id="ctl524045_label75" style="">车型Code</span>
                                </div>
                                <div id="ctl524045_Data75" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_BRAND_CDE" data-type="SheetTextBox" id="ctl524045_control75" style="">
                                </div>
                            </div>
                        </div>
                        <div class="row hidden">
                            <div class="col-md-4">
                                <div id="ctl524044_Title71" class="col-md-4">
                                    <span id="ctl524044_label71" style="">VEHICLE_TYPE_CDE</span>
                                </div>
                                <div id="ctl524044_Data71" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_TYPE_CDE" data-type="SheetTextBox" id="ctl524044_control71" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title72" class="col-md-4">
                                    <span id="ctl524044_label72" style="">VEHICLE_SUBTYPE_CDE</span>
                                </div>
                                <div id="ctl524044_Data72" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_SUBTYPE_CDE" data-type="SheetTextBox" id="ctl524044_control72" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title4" class="col-md-4">
                                    <span id="ctl524044_label4" data-type="SheetLabel" style="">ASSET_MODEL_CDE</span>
                                </div>
                                <div id="ctl524044_Data4" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MODEL_CDE" data-type="SheetTextBox" id="ctl524044_control4" style="">
                                </div>
                            </div>
                        </div>
                        <div class="row hidden">
                            <div class="col-md-4">
                                <div id="ctl524044_Title78" class="col-md-4">
                                    <span id="ctl524044_label78" style="">资产ID</span>
                                </div>
                                <div id="ctl524044_Data78" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_ID" data-type="SheetTextBox" id="ctl524044_control78" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title80" class="col-md-4">
                                    <span id="ctl524044_label80" style="">MIOCN_DSC</span>
                                </div>
                                <div id="ctl524044_Data80" class="col-md-8">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_DSC" data-type="SheetTextBox" id="ctl524044_control80" style="">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl524044_Title9" class="leftbox">
                                    <span id="ctl524044_label9" style="">资产状况</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data9" class="rightbox">
                                    <select data-datafield="VEHICLE_DETAIL.CONDITION" data-type="SheetDropDownList" id="ctl524044_control9" style=""
                                        data-masterdatacategory="资产状况" data-queryable="false" data-onchange="asset_Condition_Change(this)">
                                    </select>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title3" class="leftbox">
                                    <span id="ctl524044_label3" style="">制造商</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data3" class="rightbox">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_MAKE_DSC" disabled data-type="SheetTextBox" id="ctl524044_control3" style="width: 80%"
                                        data-schemacode="M_VEHICLE" data-querycode="fun_M_VEHICLE" data-popupwindow="PopupWindow"
                                        data-outputmappings="VEHICLE_DETAIL.ASSET_MAKE_CDE:asset_make_cde,VEHICLE_DETAIL.ASSET_MAKE_DSC:asset_make_dsc,VEHICLE_DETAIL.ASSET_BRAND_CDE:asset_brand_cde,VEHICLE_DETAIL.ASSET_BRAND_DSC:asset_brand_dsc,VEHICLE_DETAIL.ASSET_MODEL_CDE:asset_model_cde,VEHICLE_DETAIL.COMMENTS7:asset_model_dsc,VEHICLE_DETAIL.VEHICLE_COMMENT:asset_model_dsc,VEHICLE_DETAIL.MIOCN_ID:miocn_id,VEHICLE_DETAIL.MIOCN_NBR:miocn_nbr,VEHICLE_DETAIL.MIOCN_DSC:miocn_dsc,VEHICLE_DETAIL.NEW_PRICE:retail_price_amt,CONTRACT_DETAIL.TOTAL_ASSET_COST:retail_price_amt,VEHICLE_DETAIL.VEHICLE_TYPE_CDE:vehicle_type_cde,VEHICLE_DETAIL.VEHICLE_SUBTYPE_CDE:vehicle_subtyp_cde,VEHICLE_DETAIL.TRANSMISSION:transmission_type_cde">
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div id="ctl524044_Title75" class="leftbox">
                                    <span id="ctl524044_label75" style="">车型</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data75" class="rightbox">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.ASSET_BRAND_DSC" disabled data-type="SheetTextBox" id="ctl524044_control75" style="">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl524044_Title24" class="leftbox">
                                    <span id="ctl524044_label24" style="">新车指导价</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data24" class="rightbox">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.NEW_PRICE" readonly data-type="SheetTextBox" id="ctl524044_control24" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title22" class="leftbox">
                                    <span id="ctl524044_label22" style="">变速器</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data22" class="rightbox">
                                    <select data-datafield="VEHICLE_DETAIL.TRANSMISSION" data-type="SheetDropDownList" id="ctl524044_control22" style=""
                                        data-masterdatacategory="变速器" data-displayemptyitem="true" data-queryable="false">
                                    </select>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title32" class="leftbox">
                                    <span id="ctl524044_label32" style="">车辆使用年数</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data32" class="rightbox">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_AGE" data-defaultvalue="0" data-type="SheetTextBox" id="ctl524044_control32" style="">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl524044_Title77" class="leftbox">
                                    <span id="ctl524044_label77" style="">出厂日期</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data77" class="rightbox">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.RELEASE_DTE" data-type="SheetTime" id="ctl524044_control77" style=""
                                        data-defaultvalue="" data-onchange="releaseDateChange(this)">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title15" class="leftbox">
                                    <span id="ctl524044_label15" style="">里程数</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data15" class="rightbox">
                                    <input type="text" data-datafield="VEHICLE_DETAIL.ODOMETER_READING" data-type="SheetTextBox" id="ctl524044_control15" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl524044_Title7" class="leftbox">
                                    <span id="ctl524044_label7" style="">购车目的</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl524044_Data7" class="rightbox">
                                    <select data-datafield="VEHICLE_DETAIL.USAGE7" data-type="SheetDropDownList" id="ctl524044_control7" style=""
                                        data-masterdatacategory="购车目的" data-queryable="false">
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="row bottom">
                            <div class="col-md-4">
                                <div class="leftbox">
                                    担保方式
                                </div>
                                <div class="centerline"></div>
                                <div class="rightbox">
                                    <div data-datafield="VEHICLE_DETAIL.GURANTEE_OPTION_H3" data-type="SheetCheckboxList" id="ctl844175" class="" style="" data-repeatcolumns="2" data-masterdatacategory="担保方式"></div>
                                </div>
                            </div>
                            <div class="col-md-8">
                                <div id="ctl524044_Title45" class="leftbox" style="width: 11.11%">
                                    <span id="ctl524044_label45" style="">备注</span>
                                </div>
                                <div class="centerline" style="left: 17.5%;"></div>
                                <div id="ctl524044_Data45" class="rightbox" style="width: 88%">
                                    <textarea data-datafield="VEHICLE_DETAIL.COMMENTS7" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl524044_control45">											</textarea>
                                </div>
                            </div>
                        </div>
                        <div id="divZC1">
                            <div class="row">
                                <div class="col-md-8" id="powerPara">
                                    <div id="ctl524044_Title73" class="leftbox" style="width: 11.11%">
                                        <span id="ctl524044_label73" style="">动力参数</span>
                                    </div>
                                    <div class="centerline" style="left: 17.5%;"></div>
                                    <div id="ctl524044_Data73" class="rightbox" >
                                        <input type="text" data-datafield="VEHICLE_DETAIL.POWER_PARAMETER" disabled style="" data-type="SheetTextBox" id="ctl524044_control73" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title28" class="leftbox">
                                        <span id="ctl524044_label28" style="">VIN号</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data28" class="rightbox">
                                        <%--<input type="text" data-datafield="VEHICLE_DETAIL.VIN_NUMBER" readonly data-type="SheetTextBox" maxlength="17" id="ctl524044_control28"
                                                style="" data-onchange="vin_change(this)">--%>
                                        <input type="text" data-datafield="vin_number" data-type="SheetTextBox" id="ctl_vin_number" class="" style="" />
                                    </div>
                                </div>                                
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl524044_Title79" class="leftbox">
                                        <span id="ctl524044_label79" style="">MIOCN</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data79" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.MIOCN_NBR" readonly data-type="SheetTextBox" id="ctl524044_control79" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title19" class="leftbox">
                                        <span id="ctl524044_label19" style="">发动机号码</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data19" class="rightbox">
                                        <%--<input type="text" data-datafield="VEHICLE_DETAIL.ENGINE" data-type="SheetTextBox" id="ctl524044_control19" style="">--%>
                                        <input type="text" data-datafield="engine_number" data-type="SheetTextBox" id="ctl_engine_number" class="" style="" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title51" class="leftbox">
                                        <span id="ctl524044_label51" style="">车身颜色</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data51" class="rightbox">
                                        <select data-datafield="VEHICLE_DETAIL.COLOR" disabled data-type="SheetDropDownList" id="ctl524044_control51" style="" data-queryable="false"
                                            data-schemacode="M_asset_color" data-querycode="fun_M_asset_color" data-datavaluefield="COLOR_CDE" data-datatextfield="COLOR_DSC" data-displayemptyitem="true">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl524044_Title82" class="leftbox">
                                        <span id="ctl524044_label82" style="">Manufacture Year</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data82" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.MANUFACTURE_YEAR" data-type="SheetTextBox" id="ctl524044_control82" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title14" class="leftbox">
                                        <span id="ctl524044_label14" style="">TBR日期</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data14" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.BUILD_DATE" data-type="SheetTime" id="ctl524044_control14" style=""
                                            data-defaultvalue="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title16" class="leftbox">
                                        <span id="ctl524044_label16" style="">出厂月份</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data16" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.RELEASE_MONTH" readonly data-type="SheetTextBox" id="ctl524044_control16" style="">
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl524044_Title13" class="leftbox">
                                        <span id="ctl524044_label13" style="">出厂年份 </span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data13" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.RELEASE_YEAR" data-type="SheetTextBox" readonly id="ctl524044_control13" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title31" class="leftbox">
                                        <span id="ctl524044_label31" style="">注册号</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data31" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.REGISTRATION_NUMBER" data-type="SheetTextBox" id="ctl524044_control31" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title18" class="leftbox">
                                        <span id="ctl524044_label18" style="">系列</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data18" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.SERIES" data-type="SheetTextBox" id="ctl524044_control18" style="">
                                    </div>
                                </div>
                            </div>
                            <div class="row bottom">
                                <div class="col-md-4">
                                    <div id="ctl524044_Title70" class="leftbox">
                                        <span id="ctl524044_label70" style="">车身</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data70" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.VEHICLE_BODY" data-type="SheetTextBox" id="ctl524044_control70" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title61" class="leftbox">
                                        <span id="ctl524044_label61" style="">风格</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data61" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.STYLE" data-type="SheetTextBox" id="ctl524044_control61" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl524044_Title23" class="leftbox">
                                        <span id="ctl524044_label23" style="">汽缸</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl524044_Data23" class="rightbox">
                                        <input type="text" data-datafield="VEHICLE_DETAIL.CYLINDER" data-type="SheetTextBox" id="ctl524044_control23" style="">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                            <a onclick="hideInfo_New('divZC1',this)">收起 &and;</a>
                        </div>
                    </div>
                </div>
            </div>
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
            <div id="ctl508406" data-datafield="CONTRACT_DETAIL" data-type="SheetDetail" class="" style=""
                data-defaultrowcount="1" data-displayadd="false">

                <ul id="myTab_ctl508406" class="nav nav-tabs hidden">
                </ul>
                <div id="myTabContent_ctl508406" class="tab-content" style="padding: 0px">
                    <div class="template">
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl508406_Title114" class="leftbox">
                                    <span id="ctl508406_label114" style="">产品组</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data114" class="rightbox">
                                    <select data-datafield="CONTRACT_DETAIL.FP_GROUP_ID" data-type="SheetDropDownList" id="ctl508406_control114" style=""
                                        data-schemacode="M_financial_product_group" data-querycode="fun_M_financial_product_group" data-datavaluefield="FP_GROUP_ID" data-datatextfield="FP_GROUP_DSC" data-displayemptyitem="true">
                                    </select>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508406_Title4" class="leftbox">
                                    <span id="ctl508406_label4" style="">产品类型</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data4" class="rightbox">
                                    <select data-datafield="CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID" data-type="SheetDropDownList" id="ctl508406_control4" style=""
                                        data-schemacode="M_FinancialType" data-querycode="fun_M_FinancialType" data-datavaluefield="financial_product_id" data-datatextfield="financial_product_dsc" data-displayemptyitem="true"
                                        data-filter="VEHICLE_DETAIL.CONDITION:asset_condition,APPLICATION_TYPE_CODE:type,bp_id:bp_id,CONTRACT_DETAIL.FP_GROUP_ID:group_id,VEHICLE_DETAIL.ASSET_MODEL_CDE:model_cde">
                                    </select>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508406_Title95" class="leftbox">
                                    <span id="ctl508406_label95" style="">付款频率</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data95" class="rightbox">
                                    <select data-datafield="CONTRACT_DETAIL.FREQUENCY_CDE" data-type="SheetDropDownList" id="ctl508406_control95" style=""
                                        data-masterdatacategory="付款频率">
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl508406_Title27" class="leftbox">
                                    <span id="ctl508406_label27" style="">贷款期数（月）</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data27" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.LEASE_TERM_IN_MONTH" data-type="SheetTextBox" id="ctl508406_control27" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508407_Title126" class="leftbox">
                                    <span id="ctl508407_label126" style="">附加费</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508407_Data126" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.TOTAL_ACCESSORY_AMT" data-type="SheetTextBox" id="ctl508407_control126" style="color: red"
                                        data-computationrule="{CONTRACT_DETAIL.ACCESSORY_AMT}">
                                    <a href="#accessoryDetail" data-toggle="modal" onclick="" target="_blank">详细</a>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="title3571" class="leftbox">
                                    敞口金额
                                </div>
                                <div class="centerline"></div>
                                <div id="control3571" class="rightbox">
                                    <span id="ctrlCkje" style="color: red;"></span>
                                    <a href="#example" data-toggle="modal" onclick="" target="_blank">详细</a>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl508406_Title10" class="leftbox">
                                    <span id="ctl508406_label10" style="">合同价格</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data10" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.ASSET_COST" data-type="SheetTextBox" disabled id="ctl508406_control10" style=""
                                        data-computationrule="{CONTRACT_DETAIL.SALE_PRICE}" data-formatrule="{0:N2}">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508406_Title75" class="leftbox">
                                    <span id="ctl508406_label75" style="">首付款比例%</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data75" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT" data-type="SheetTextBox" id="ctl508406_control75" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508406_Title14" class="leftbox">
                                    <span id="ctl508406_label14" style="">首付款金额</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data14" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.CASH_DEPOSIT" data-type="SheetTextBox" id="ctl508406_control14" style="" data-formatrule="{0:N2}">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div id="ctl508407_Title27" class="leftbox">
                                    <span id="ctl508407_label27" style="">销售价格</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508407_Data27" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.TOTAL_ASSET_COST" data-type="SheetTextBox" id="ctl508407_control27" style=""
                                        data-formatrule="{0:N2}">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508406_Title89" class="leftbox">
                                    <span id="ctl508406_label89" style="">贷款额比例 %</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data89" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.FINANCED_AMT_PCT" disabled data-type="SheetTextBox" id="ctl508406_control89" style="">
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div id="ctl508406_Title21" class="leftbox">
                                    <span id="ctl508406_label21" style="">贷款金额</span>
                                </div>
                                <div class="centerline"></div>
                                <div id="ctl508406_Data21" class="rightbox">
                                    <input type="text" data-datafield="CONTRACT_DETAIL.AMOUNT_FINANCED" disabled data-type="SheetTextBox" id="ctl508406_control21" style="" data-formatrule="{0:N2}">
                                </div>
                            </div>
                        </div>
                        <div class="row tableContent bottom">
                            <div id="title359" class="leftbox1" style="width: 11.666666%;">
                                <span id="Label341" data-type="SheetLabel" data-datafield="RentalDetailtable" style="">还款计划</span>
                            </div>
                            <div id="control359" class="rightbox1">
                                <table id="ctl620410" data-datafield="PMS_RENTAL_DETAIL" data-type="SheetGridView" class="SheetGridView" style=""
                                    data-defaultrowcount="0" data-displayadd="false">
                                    <tbody>
                                        <tr class="header">
                                            <td class="rowSerialNo">序号</td>
                                            <td style="" class="hidden">RENTAL_DETAIL_SEQ</td>
                                            <td style="">还款起始期</td>
                                            <td style="">还款结束期</td>
                                            <td style="">还款额</td>
                                            <td style="">每期还款总额</td>
                                            <td style="" class="hidden">INTEREST_RTE</td>
                                        </tr>
                                        <tr class="template">
                                            <td class=""></td>
                                            <td id="ctl620410_td1" data-datafield="PMS_RENTAL_DETAIL.RENTAL_DETAIL_SEQ" style="" class="hidden">
                                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.RENTAL_DETAIL_SEQ" data-type="SheetTextBox" id="ctl620410_control1" style=""></td>
                                            <td id="ctl620410_td3" data-datafield="PMS_RENTAL_DETAIL.START_TRM" style="">
                                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.START_TRM" data-type="SheetTextBox" id="ctl620410_control3" style="text-align: center"></td>
                                            <td id="ctl620410_td4" data-datafield="PMS_RENTAL_DETAIL.END_TRM" style="">
                                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.END_TRM" data-type="SheetTextBox" id="ctl620410_control4" style="text-align: center"></td>
                                            <td id="ctl620410_td5" data-datafield="PMS_RENTAL_DETAIL.RENTAL_AMT" style="">
                                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.RENTAL_AMT" data-type="SheetTextBox" id="ctl620410_control5" style="text-align: center" data-formatrule="{0:N2}"></td>
                                            <td id="ctl620410_td6" data-datafield="PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT" style="">
                                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT" data-type="SheetTextBox" id="ctl620410_control6" style="text-align: center" data-formatrule="{0:N2}"></td>
                                            <td id="ctl620410_td7" data-datafield="PMS_RENTAL_DETAIL.INTEREST_RTE" style="" class="hidden">
                                                <input disabled type="text" data-datafield="PMS_RENTAL_DETAIL.INTEREST_RTE" data-type="SheetTextBox" id="ctl620410_control7" style=""></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div id="divJRTK1">
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl508406_Title157" class="leftbox">
                                        <span id="ctl508406_label157" style="">展期期数</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data157" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.DEFERRED_TRM" disabled data-type="SheetTextBox" id="ctl508406_control157" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl508406_Title23" class="leftbox">
                                        <span id="ctl508406_label23" style="">资产残值/轻松融资尾款%</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data23" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.BALLOON_PERCENTAGE" data-type="SheetTextBox" disabled id="ctl508406_control23" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl508406_Title25" class="leftbox">
                                        <span id="ctl508406_label25" style="">资产残值/轻松融资尾款金额</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data25" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.BALLOON_AMOUNT" data-type="SheetTextBox" disabled id="ctl508406_control25" style="">
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl508406_Title124" class="leftbox">
                                        <span id="ctl508406_label124" style="">客户利率%</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data124" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.BASE_CUSTOMER_RATE" disabled data-type="SheetTextBox" id="ctl508406_control124" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl508406_Title140" class="leftbox">
                                        <span id="ctl508406_label140" style="">贴息金额</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data140" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.CALC_SUBSIDY_AMT" disabled data-type="SheetTextBox" id="ctl508406_control140" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl508406_Title141" class="leftbox">
                                        <span id="ctl508406_label141" style="">实际利率%</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data141" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.ACTUAL_RTE" disabled data-type="SheetTextBox" id="ctl508406_control141" style="">
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl508406_Title62" class="leftbox">
                                        <span id="ctl508406_label62" style="">利息总额</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data62" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.ASSETITC" disabled data-type="SheetTextBox" id="ctl508406_control62" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl508406_Title139" class="leftbox">
                                        <span id="ctl508406_label139" style="">贴息利率%</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508406_Data139" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.CALC_SUBSIDY_RTE" disabled data-type="SheetTextBox" id="ctl508406_control139" style="">
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="ctl508407_Title124" class="leftbox">
                                        <span id="ctl508407_label124" style="">应付总额</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508407_Data124" class="rightbox">
                                        <input type="text" data-datafield="CONTRACT_DETAIL.YFJE" data-type="SheetTextBox" id="ctl508407_control124" style=""
                                            disabled>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div id="ctl508407_Title62" class="leftbox">
                                        <span id="ctl508407_label62" style="">未偿余额</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="ctl508407_Data62" class="rightbox">
                                        <%--Netsol没有保存到数据库--%>
                                        <input type="text" data-datafield="CONTRACT_DETAIL.WCYE" data-type="SheetTextBox" id="ctl508407_control62" style=""
                                            disabled>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="title407" class="leftbox">
                                        <span id="Label366" data-type="SheetLabel" data-datafield="Bankname" style="">客户银行名称</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="control407" class="rightbox">
                                        <input data-datafield="Bankname" data-type="SheetDropDownList" id="ctl64724" class="" style="" data-masterdatacategory="银行" data-schemacode="yhld" data-querycode="yhld" data-filter="0:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div id="title408" class="leftbox">
                                        <span id="Label367" data-type="SheetLabel" data-datafield="Branchname" style="">客户银行分支</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="control408" class="rightbox">
                                        <input data-datafield="Branchname" data-type="SheetDropDownList" id="ctl875733" class="" style="" data-schemacode="yhld" data-querycode="yhld" data-filter="Bankname:PARENTCODE" data-datavaluefield="BANKCODE" data-datatextfield="BANKNAME" />
                                    </div>
                                </div>
                            </div>
                            <div class="row bottom">
                                <div class="col-md-4">
                                    <div id="title409" class="leftbox">
                                        <span id="Label368" data-type="SheetLabel" data-datafield="Accoutname" style="">客户账户名</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="control409" class="rightbox">
                                        <input id="Control368" type="text" data-datafield="Accoutname" data-type="SheetTextBox" style="">
                                    </div>
                                </div>


                                <div class="col-md-4">
                                    <div id="title410" class="leftbox">
                                        <span id="Label369" data-type="SheetLabel" oninput="textnum(this)" data-datafield="AccoutNumber" style="">客户账户号</span>
                                    </div>
                                    <div class="centerline"></div>
                                    <div id="control410" class="rightbox">
                                        <input id="Control369" type="text" data-datafield="AccoutNumber" data-type="SheetTextBox" style="">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div data-isretract="true" style="position: absolute; right: 0; bottom: 0px; width: 10%; height: 40px; line-height: 40px; text-align: center;">
                            <a onclick="hideInfo_New('divJRTK1',this)">收起 &and;</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%-------------------客户测算-------------------------------%>
        <div class="nav-icon fa  fa-angle-double-down ss" style="width: 90%; margin-top: 10px; margin-bottom: 10px; border-bottom: 1px solid #ccc;">
            <label data-en_us="Sheet information">客户测算</label>
        </div>
        <div style="position: relative; padding-right: 10%;" id="divCustoms">
            <div class="row tableContent template">
                <div id="title_pcm" class="leftbox1" style="width: 11.666666%;">
                    <span></span>
                    <br />
                    <span></span>
                </div>
                <div class="rightbox1">
                    <table id="table_pcm" data-type="SheetGridView" class="SheetGridView">
                        <tbody>
                            <tr class="header">
                                <td id="ControIncomOfMonth_Header">
                                    <label data-type="SheetLabel" style="">月收入估值</label>
                                </td>
                                <td id="ControDabtsOfMonth_Heade">
                                    <label data-type="SheetLabel" style="">月应还债务</label>
                                </td>
                                <td id="ControAssetValuation_Header">
                                    <label data-type="SheetLabel" style="">客户资产估值</label>
                                </td>
                                <td id="ControRepayLoan_Header">
                                    <label data-type="SheetLabel" style="">客户月还贷能力</label>
                                </td>
                            </tr>
                            <tr class="c_row">
                                <td>
                                    <label data-data="C_IncomOfMonth" style="text-align: center"></label>
                                </td>
                                <td>
                                    <label data-data="C_DabtsOfMonth" style="text-align: center"></label>
                                </td>
                                <td>
                                    <label data-data="C_AssetValuation" style="text-align: center"></label>
                                </td>
                                <td>
                                    <label data-data="C_RepayLoan" style="text-align: center"></label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
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
                            <input id="Control384" type="text" data-datafield="Dfgz" data-type="SheetTextBox" style="" maxlength="9">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title432" class="leftbox2">
                            <span id="Label387" data-type="SheetLabel" data-datafield="Hqjx" style="">活期结息</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control432" class="rightbox2">
                            <input id="Control387" type="text" data-datafield="Hqjx" data-type="SheetTextBox" style="" maxlength="9">
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
                            <input id="Control390" type="text" data-datafield="Bccdygje" data-type="SheetTextBox" style="" maxlength="9">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title436" class="leftbox2">
                            <span id="Label391" data-type="SheetLabel" data-datafield="QTFZYG" style="">其他负债月供</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control436" class="rightbox2">
                            <input id="Text1" type="text" data-datafield="QTFZYG" data-type="SheetTextBox" style="" maxlength="9">
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
                            <input id="Control111" type="text" data-datafield="Financialratiodes" data-type="SheetTextBox" style="" maxlength="9">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title112" class="leftbox3">
                            <span id="Label112" data-type="SheetLabel" data-datafield="Evalresult" style="">财务比率金额</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control112" class="rightbox3">
                            <input id="Control112" type="text" data-datafield="Evalresult" data-type="SheetTextBox" style="" maxlength="9">
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
                            <input id="Control108" type="text" data-datafield="Descritption" data-type="SheetTextBox" style="" maxlength="9">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title430" class="leftbox2">
                            <span id="Label385" data-type="SheetLabel" data-datafield="Dqck" style="">定期存款</span>
                        </div>
                        <div class="centerline2"></div>
                        <div id="control430" class="rightbox2">
                            <input id="Control385" type="text" data-datafield="Dqck" data-type="SheetTextBox" style="" maxlength="9">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title431" class="leftbox3">
                            <span id="Label386" data-type="SheetLabel" data-datafield="Gp" style="">股票</span>
                        </div>
                        <div class="centerline3"></div>
                        <div id="control431" class="rightbox3">
                            <input id="Control386" type="text" data-datafield="Gp" data-type="SheetTextBox" style="" maxlength="9">
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
                            <input id="Control388" type="text" data-datafield="Gj" data-type="SheetTextBox" style="" maxlength="9">
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div id="title434" class="leftbox2">
                            <span id="Label389" data-type="SheetLabel" data-datafield="Lc" style="">理财</span>
                        </div>
                        <div class="centerline"></div>
                        <div id="control434" class="rightbox2">
                            <input id="Control389" type="text" data-datafield="Lc" data-type="SheetTextBox" style="" maxlength="9">
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
                <div id="Control342" data-datafield="SQD" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
                </div>
            </div>
        </div>
        <div class="row">
            <div id="title375" class="col-md-2">
                <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
            </div>
            <div id="control375" class="col-md-10">
                <div id="Control349" data-datafield="ZX" data-type="SheetAttachment" style="" data-allowbatchdownload="false"></div>
            </div>
        </div>
        <div class="row">
            <div id="title363" class="col-md-2">
                <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
            </div>
            <div id="control363" class="col-md-10">
                <div id="Control343" data-datafield="SFZ" data-type="SheetAttachment" style="" data-allowbatchdownload="false"></div>
            </div>
        </div>
        <div class="row">
            <div id="title367" class="col-md-2">
                <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
            </div>
            <div id="control367" class="col-md-10">
                <div id="Control345" data-datafield="JSZ" data-type="SheetAttachment" style="" data-allowbatchdownload="false"></div>
            </div>
        </div>
        <div class="row">
            <div id="title365" class="col-md-2">
                <span id="Label344" data-type="SheetLabel" data-datafield="JHZ" style="">婚姻类材料</span>
            </div>
            <div id="control365" class="col-md-10">
                <div id="Control344" data-datafield="JHZ" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
                </div>
            </div>
        </div>
        <div class="row">
            <div id="title369" class="col-md-2">
                <span id="Label346" data-type="SheetLabel" data-datafield="JZZ" style="">居住类材料</span>
            </div>
            <div id="control369" class="col-md-10">
                <div id="Control346" data-datafield="JZZ" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
                </div>
            </div>
        </div>
        <div class="row">
            <div id="title373" class="col-md-2">
                <span id="Label348" data-type="SheetLabel" data-datafield="GZ" style="">工作证明\企业类证明</span>
            </div>
            <div id="control373" class="col-md-10">
                <div id="Control348" data-datafield="GZ" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
                </div>
            </div>
        </div>
        <div class="row">
            <div id="title371" class="col-md-2">
                <span id="Label347" data-type="SheetLabel" data-datafield="SR" style="">收入类材料</span>
            </div>
            <div id="control371" class="col-md-10">
                <div id="Control347" data-datafield="SR" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
                </div>
            </div>
        </div>
        <div class="row">
            <div id="title391" class="col-md-2">
                <span id="Label357" data-type="SheetLabel" data-datafield="YHKMFYJ" style="">客户银行卡面复印件</span>
            </div>
            <div id="control391" class="col-md-10">
                <div id="Control357" data-datafield="YHKMFYJ" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
                </div>
            </div>
        </div>
        <div class="row">
            <div id="title471" class="col-md-2">
                <span id="Label420" data-type="SheetLabel" data-datafield="xsqtfj" style="">信审其他附件</span>
            </div>
            <div id="control471" class="col-md-10">
                <div id="Control420" data-datafield="xsqtfj" data-type="SheetAttachment" style="" data-allowbatchdownload="false">
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
            <div class="col-md-2">
                <span data-type="SheetLabel" data-datafield="PXJKC" style="">平行进口车</span>
            </div>
            <div class="col-md-10">
                <div data-datafield="PXJKC" data-type="SheetAttachment" style="">
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-2">
                <span id="Label35711" data-type="SheetLabel" data-datafield="jfbg" style="">家访报告</span>
            </div>
            <div class="col-md-10">
                <div id="Control35711" data-datafield="jfbg" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
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
                <div class="row tableContent" style="border-right: 0px solid #ccc; border-top: 0px solid #ccc;">
                    <div id="control509" class="col-md-10" style="border-left: 0px solid #ccc; width: 99%">
                        <table id="Control145" data-datafield="ASSET_ACCESSORY" data-type="SheetGridView" class="SheetGridView">
                            <tbody>
                                <tr class="header">
                                    <td id="Control145_SerialNo" class="rowSerialNo">序号								</td>
                                    <td id="Control145_Header3" data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE">
                                        <label id="Control145_Label3" data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE" data-type="SheetLabel" style="">选装部件</label>
                                    </td>
                                    <td id="Control145_Header4" data-datafield="ASSET_ACCESSORY.PRICE" style="width: 180px">
                                        <label id="Control145_Label4" data-datafield="ASSET_ACCESSORY.PRICE" data-type="SheetLabel" style="">价格</label>
                                    </td>
                                    <td class="rowOption">删除								</td>
                                </tr>
                                <tr class="template">
                                    <td id="Control145_Option" class="rowOption"></td>
                                    <td data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE">
                                        <select id="Control145_ctl3" data-datafield="ASSET_ACCESSORY.ACCESSORY_CDE" data-type="SheetDropDownList" style="width: 92%" data-displayemptyitem="true"
                                            data-schemacode="M_accessory_code" data-querycode="fun_M_accessory_code" data-datavaluefield="ACCESSORY_CDE" data-datatextfield="ACCESSORY_DSC">
                                        </select>
                                    </td>
                                    <td data-datafield="ASSET_ACCESSORY.PRICE">
                                        <input id="Control145_ctl4" type="text" data-datafield="ASSET_ACCESSORY.PRICE" data-type="SheetTextBox" style="width: 92%">
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
            <ul id="exposure_Tab" class="nav nav-tabs">
            </ul>
            <div id="exposure_TabContent" class="tab-content">
                <div class="template">
                    <div class="title">CAP：风险敞口<span class="title_total">总风险:<span id="sub_total_cap"></span></span></div>
                    <div class="divBody">
                        <table class="SheetGridView" style="width: 1500px;">
                            <thead>
                                <tr class="header">
                                    <td id="" class="rowSerialNo"></td>
                                    <td id="Td1">融资额
                                    </td>
                                    <td id="Td2">申请号
                                    </td>
                                    <td id="Td3">状态
                                    </td>
                                    <td id="Td4">角色
                                    </td>
                                    <td id="Td5">ID/注册号
                                    </td>
                                    <td id="Td6">申请人姓名
                                    </td>
                                    <td id="Td7">融资类型
                                    </td>
                                    <td id="Td8">类型
                                    </td>
                                    <td id="Td20">状态日期
                                    </td>
                                    <td id="Td21">期数
                                    </td>
                                    <td id="Td22">生产商
                                    </td>
                                    <td id="Td23">动力参数
                                    </td>
                                    <td id="Td24">车型
                                    </td>
                                </tr>
                            </thead>
                            <tbody id="Application" style="text-align: center;"></tbody>
                        </table>
                    </div>
                    <div class="title">CMS：风险敞口<span class="title_total">总风险:<span id="sub_total_cms"></span></span></div>
                    <div class="divBody">
                        <table class="SheetGridView" style="width: 2400px;">
                            <thead>
                                <tr class="header">
                                    <td id="Td9" class="rowSerialNo"></td>
                                    <td id="Td10">本金余额
                                    </td>
                                    <td id="Td11">申请号
                                    </td>
                                    <td id="Td12">合同号
                                    </td>
                                    <td id="Td13">状态
                                    </td>
                                    <td id="Td14">申请人状态
                                    </td>
                                    <td id="Td15">角色
                                    </td>
                                    <td id="Td16">ID/注册号
                                    </td>
                                    <td id="Td17">姓名
                                    </td>
                                    <td id="Td18">经销商
                                    </td>
                                    <td id="Td19">合同类型
                                    </td>
                                    <td id="Td25">合同日期
                                    </td>
                                    <td id="Td26">利率
                                    </td>
                                    <td id="Td27">融资额
                                    </td>
                                    <td id="Td28">生产商
                                    </td>
                                    <td id="Td29">动力参数
                                    </td>
                                    <td id="Td30">车型
                                    </td>
                                    <td id="Td31">30天
                                    </td>
                                    <td id="Td32">60天
                                    </td>
                                    <td id="Td33">90天
                                    </td>
                                    <td id="Td34">90天以上
                                    </td>
                                    <td id="Td35">120天以上
                                    </td>
                                    <td id="Td36">期数
                                    </td>
                                    <td id="Td37">已付期数
                                    </td>
                                </tr>
                            </thead>
                            <tbody id="cmsApplication" style="text-align: center;"></tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="container">
                <div class="row bottom">
                    <div class="col-md-3">
                        <div class="col-md-6">总敞口金额：</div>
                        <div class="col-md-6">
                            <span id="fullckje"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="col-md-6">CAP敞口金额：</div>
                        <div class="col-md-6">
                            <span id="capckje"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="col-md-6">CMS敞口金额：</div>
                        <div class="col-md-6">
                            <span id="cmsckje"></span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="col-md-6">或有负债：</div>
                        <div class="col-md-6">
                            <span id="fz_amt"></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal-footer">
            <a class="btn" data-dismiss="modal">关闭</a>
        </div>
    </div>

    <div id="dealersDetail" class="modal fade in" aria-hidden="true">
        <div class="modal-header">
            <a class="close" data-dismiss="modal">×</a>
            <h3 style="text-align: center;">单位敞口</h3>

        </div>
        <div class="modal-body">
            <div style="height: auto; min-height: 90px; max-height: 400px; overflow-y: auto;">
                <div class="row tableContent" style="border-right: 0px solid #ccc; border-top: 0px solid #ccc;">
                    <div id="control510" class="col-md-10" style="border-left: 0px solid #ccc; width: 99%">
                        <table id="Control146" data-type="SheetGridView" class="SheetGridView">
                            <thead>
                                <tr class="header">
                                    <td class="rowSerialNo hidden">序号</td>
                                    <td>申请单号</td>
                                    <td>公司名称</td>
                                    <td>申请日期</td>
                                    <td>初审审核状态</td>
                                    <td>终审审核状态</td>
                                    <td>申请状态</td>
                                </tr>
                            </thead>
                            <tbody id="dealersInfo">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
