<%@ Page Language="C#" AutoEventWireup="true" CodeFile="yyys.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.yyys" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <link type="text/css" href="../../../../jQueryViewer/css/viewer.min.css" rel="stylesheet" />
    <script src="../../../../jQueryViewer/js/viewer.min.js" type="text/javascript"></script>
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
        }

        #divattachment .fa-download {
            display: none !important;
        }

        #divattachment #Control429 .fa-download {
            display: inline-block !important;
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

        .chkbank select {
            display: block !important;
        }

        .chkbank span {
            display: none !important;
        }
    </style>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">零售贷款申请</label>
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
                    <span id="Label11" data-type="SheetLabel" data-datafield="APPLICATION_NUMBER" style="">贷款申请号码</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input id="Control11" type="text" data-datafield="APPLICATION_NUMBER" data-type="SheetTextBox" style="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="APPLICATION_TYPE_CODE" style="">申请类型</span>
                </div>
                <div id="control2" class="col-md-4">
                    <select id="Control58" data-datafield="APPLICATION_TYPE_CODE" data-type="SheetDropDownList" style="" data-masterdatacategory="申请类型" data-queryable="false"></select>
                </div>
            </div>
            <div class="row hidden">
                <div id="title40000" class="col-md-2">
                    bp_id
                </div>
                <div id="control40000" class="col-md-4">
                    <input type="text" id="hidden_001" data-datafield="bp_id" data-type="SheetTextBox" style="" />
                </div>
            </div>
            <div class="row">
                <div id="title4" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="BP_COMPANY_ID" style="">申请贷款公司</span>
                </div>
                <div id="control4" class="col-md-4">
                    <select id="Control14" data-datafield="BP_COMPANY_ID" data-type="SheetDropDownList" data-masterdatacategory="申请贷款公司" style="" data-queryable="false"></select>
                </div>
                <div id="title13" class="col-md-2">
                    <span id="Label23" data-type="SheetLabel" data-datafield="REFERENCE_NBR" style="">申请参考号</span>
                </div>
                <div id="control13" class="col-md-4">
                    <input id="Control23" type="text" data-datafield="REFERENCE_NBR" data-type="SheetTextBox" style="" onkeyup="this.value=strMaxLength(this.value,'20')">
                </div>
            </div>
            <div class="row">
                <div id="title14" class="col-md-2">
                    <span id="Label24" data-type="SheetLabel" data-datafield="BUSINESS_PARTNER_ID" style="">经销商</span>
                </div>
                <div id="Div1" class="col-md-4">
                    <select id="Control24" data-datafield="BUSINESS_PARTNER_ID" data-type="SheetDropDownList" style="" data-queryable="false"
                        data-schemacode="M_business_partner_id" data-querycode="003" data-datavaluefield="BUSINESS_PARTNER_ID" data-datatextfield="BUSINESS_PARTNER_NME" data-filter="bp_id:bp_id">
                    </select>
                </div>
                <div id="title15" class="col-md-2">
                    <span id="Label25" data-type="SheetLabel" data-datafield="USER_NAME" style="">账户名</span>
                </div>
                <div id="control15" class="col-md-4">
                    <input id="Control25" type="text" data-datafield="USER_NAME" data-type="SheetTextBox" style="">
                </div>

            </div>
            <div class="row" cus_type="sqr">
                <div id="title21" class="col-md-2">
                    <label>
                        <span id="Label29" data-type="SheetLabel" style="">姓名（中文）</span>
                    </label>
                </div>
                <div id="sqr_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div id="title23" class="col-md-2">
                    <label>
                        <span id="Label31" data-type="SheetLabel" style="">证件号码</span>
                    </label>
                </div>
                <div id="sqr_id" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="sqr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">公司名称</span>
                    </label>
                </div>
                <div id="sqr_company_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">组织机构代码</span>
                    </label>
                </div>
                <div id="sqr_company_org_code" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="sqr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">注册号</span>
                    </label>
                </div>
                <div id="sqr_company_sts" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="gjr">
                <div id="title123" class="col-md-2">
                    <label>
                        <span id="Label122" data-type="SheetLabel" style="">共借人姓名（中文）</span>
                    </label>
                </div>
                <div id="gjr_name" class="col-md-4">
                    <label><span></span></label>
                </div>
                <div id="title125" class="col-md-2">
                    <label>
                        <span id="Label124" style="">共借人证件号码</span>
                    </label>
                </div>
                <div id="gjr_id" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="gjr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">共借公司名称</span>
                    </label>
                </div>
                <div id="gjr_company_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">共借公司组织机构代码</span>
                    </label>
                </div>
                <div id="gjr_company_org_code" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="gjr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">共借公司注册号</span>
                    </label>
                </div>
                <div id="gjr_company_sts" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="dbr">
                <div id="title217" class="col-md-2">
                    <label>
                        <span id="Label209" data-type="SheetLabel" style="">担保人姓名</span>
                    </label>
                </div>
                <div id="dbr_name" class="col-md-4">
                    <label><span></span></label>
                </div>
                <div id="title219" class="col-md-2">
                    <label>
                        <span id="Label211" data-type="SheetLabel" style="">担保人证件号码</span>
                    </label>
                </div>
                <div id="dbr_id" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="dbr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">担保公司名称</span>
                    </label>
                </div>
                <div id="dbr_company_name" class="col-md-4">
                    <label><span></span></label>
                </div>

                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">担保公司组织机构代码</span>
                    </label>
                </div>
                <div id="dbr_company_org_code" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row" cus_type="dbr_company">
                <div class="col-md-2">
                    <label>
                        <span data-type="SheetLabel" style="">担保公司注册号</span>
                    </label>
                </div>
                <div id="dbr_company_sts" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>
            <div class="row">
                <div id="title421" class="col-md-2">
                    <span id="Label379" data-type="SheetLabel"  style="">风控审核结果</span>
                </div>
                <div id="control421" class="col-md-4">
                    
                </div>
                <div id="Div5" class="col-md-2">
                    <label>
                        <span id="Span3" data-type="SheetLabel" style="">申请人电话</span>
                    </label>
                </div>
                <div id="sqr_mobile" class="col-md-4">
                    <label><span></span></label>
                </div>
            </div>

            <div class="row">
                <div id="title514" class="col-md-2">
                    <span id="Label448" data-type="SheetLabel" data-datafield="dysf" style="">抵押省份</span>
                </div>
                <div id="control514" class="col-md-4">
                    <select data-datafield="dysf" data-type="SheetDropDownList" id="ctl699489" class="" style="" data-displayemptyitem="true" data-schemacode="area" data-querycode="area" data-filter="100000:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME">
                    </select>
                </div>
                <div id="title515" class="col-md-2">
                    <span id="Label449" data-type="SheetLabel" data-datafield="dycs" style="">抵押城市</span>
                </div>
                <div id="control515" class="col-md-4">
                    <select data-datafield="dycs" data-type="SheetDropDownList" id="ctl53508" class="" style="" data-displayemptyitem="true" data-schemacode="area" data-querycode="area" data-filter="dysf:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="title516" class="col-md-2">
                    <span id="Label450" data-type="SheetLabel" data-datafield="dyx" style="">抵押县</span>
                </div>
                <div id="control516" class="col-md-4">
                    <select data-datafield="dyx" data-type="SheetDropDownList" id="ctl291016" class="" style="" data-displayemptyitem="true" data-schemacode="area" data-querycode="area" data-filter="dycs:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME">
                    </select>
                </div>
                <div id="Div8" class="col-md-2">
                </div>
                <div id="Div9" class="col-md-4">
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
            <div class="row tableContent">
                <div id="title425" class="leftbox1">
                    <span id="Label382" data-type="SheetLabel" data-datafield="THJL" style="">电调记录</span>
                </div>
                <div id="control425" class="rightbox1">
                    <textarea id="Control382" data-datafield="THJL" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" id="divattachmentinfo" onclick="hidediv('divattachment',this)">
            <label id="Label1" data-en_us="Sheet information">附件信息</label>
            <a href="javascript:void(0);" onclick="getDownLoadURL()">附件详情</a>
        </div>
        <div class="divContent" id="divattachment">
            <div id="divfjll">
                <ul id="ulfjll">
                </ul>
            </div>
            <div>
                <div class="row">
                    <div id="title383" class="col-md-2">
                        <span id="Label353" data-type="SheetLabel" data-datafield="DKHT" style="">贷款合同</span>
                    </div>
                    <div id="control383" class="col-md-10">
                        <div id="Div7" data-datafield="DKHT" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title363" class="col-md-2">
                        <span id="Label343" data-type="SheetLabel" data-datafield="SFZ" style="">身份证</span>
                    </div>
                    <div id="control363" class="col-md-10">
                        <div id="Div4" data-datafield="SFZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title361" class="col-md-2">
                        <span id="Label342" data-type="SheetLabel" data-datafield="SQD" style="">申请单资料</span>
                    </div>
                    <div id="control361" class="col-md-10">
                        <div id="Div13" data-datafield="SQD" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div id="title367" class="col-md-2">
                        <span id="Label345" data-type="SheetLabel" data-datafield="JSZ" style="">驾驶类资料</span>
                    </div>
                    <div id="control367" class="col-md-10">
                        <div id="Div15" data-datafield="JSZ" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div id="title375" class="col-md-2">
                        <span id="Label349" data-type="SheetLabel" data-datafield="ZX" style="">征信授权书</span>
                    </div>
                    <div id="control375" class="col-md-10">
                        <div id="Div19" data-datafield="ZX" data-type="SheetAttachment" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
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
                        <span id="Label35711" data-type="SheetLabel" data-datafield="jfbg" style="">家访报告</span>
                    </div>
                    <div class="col-md-10">
                        <div id="Control35711" data-datafield="jfbg" data-type="SheetAttachment" data-picturecompress="true" style="" data-fileextensions=".jpg,.gif,.png,.bmp,.jpeg,.pdf">
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div id="title497" class="col-md-2">
                        <span id="Label437" data-type="SheetLabel" data-datafield="fjfiqt" style="">FI其他附件</span>
                    </div>
                    <div id="control497" class="col-md-10">
                        <div id="Control437" data-datafield="fjfiqt" data-type="SheetAttachment" style="">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-down bannerTitle" onclick="hidediv('div2',this)">
            <label id="Label2" data-en_us="Sheet information">审核信息</label>
        </div>
        <div class="divContent" id="div20">
            <div class="row tableContent">
                <div id="Div21" class="col-md-2">
                    <span id="Span1" data-type="SheetLabel" data-datafield="CSYJ" style="">运营预审意见</span>
                </div>
                <div id="Div22" class="col-md-10">
                    <div data-datafield="yyysshzt" data-type="SheetRadioButtonList" id="Div23" class="" style="" data-defaultitems="核准;拒绝;"></div>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title399" class="col-md-2">
                    <span id="Label361" data-type="SheetLabel" data-datafield="CSYJ" style="">运营初审意见</span>
                </div>
                <div id="control399" class="col-md-10">
                    <div data-datafield="yycsshzt" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultitems="核准;拒绝;"></div>

                </div>
            </div>
            <div class="row tableContent">
                <div id="Div24" class="col-md-2">
                    <span id="Span2" data-type="SheetLabel" data-datafield="ZSYJj" style="">运营终审意见</span>
                </div>
                <div id="Div25" class="col-md-10">
                    <div data-datafield="yyzsshzt" data-type="SheetRadioButtonList" id="Div26" class="" style="" data-defaultitems="核准;拒绝;"></div>
                </div>
            </div>
        </div>
    </div>
    <script src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>" type="text/javascript"></script>
    <script type="text/javascript">
        //隐藏附加上传空白行
        function hideFIrow() {
            $("#divattachment .row").each(function () {
                if ($(this).find(">.col-md-2>span").css("display") == "none") {
                    $(this).hide();
                }
            })
        }
        var viewer;
        // 页面加载完成事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
            var IsWork = $.MvcSheetUI.QueryString("Mode");
            //风控审核结果转换
            var sysType = $.MvcSheetUI.SheetInfo.BizObject.DataItems["FK_SYS_TYPE"].V; 

            if (sysType == "1") {
                var fkresult = $.MvcSheetUI.SheetInfo.BizObject.DataItems["FK_RESULT"].V; 

                if (fkresult == null || fkresult == "" || fkresult == undefined) {
                    fkresult = "";
                }

                $("#control421").append("<span style='color:red'>" + fkresult + "</span>");
            }
            else {
                var fkresult = $.MvcSheetUI.SheetInfo.BizObject.DataItems["fkResult"].V; 
                var arrrsrq = []
                arrrsrq['localreject'] = "东正本地规则<span style=\"color:red;\">拒绝</span>";
                arrrsrq['cloudaccept'] = "云端规则<span style=\"color:red;\">通过</span>";
                arrrsrq['cloudreject'] = "云端规则<span style=\"color:red;\">拒绝</span>";
                arrrsrq['cloudmanual'] = "云端规则返回<span style=\"color:red;\">转人工</span>";
                arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
                $("#control421").append(arrrsrq[fkresult]);
            }
            
            hideFIrow();
            getmsg();
            //添加留言
            $('#addmsga').on('click', function () { addmsg(); });
            document.oncontextmenu = function () {
                return false;
            }
            getApplicantUserInfoByBizObject();
        }
    </script>
</asp:Content>
