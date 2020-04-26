<%@ Page Language="C#" AutoEventWireup="true" CodeFile="YSP.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.YSP" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <link href="css/common.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" rel="stylesheet" />
    <link href="css/FIApplication.css?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010" rel="stylesheet" />
    <script type="text/javascript" src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <script type="text/javascript" src="js/Validate.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <script type="text/javascript" src="/Portal/Custom/js/ocr.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>010"></script>
    <script type="text/javascript">
        // 控件初始化事件
        $.MvcSheet.ControlRendered = function () {
            // 如果是 SheetComment，则默认设置所有的 SheetComment 的属性\
            var sheetmode = $.MvcSheetUI.SheetInfo.SheetMode;
            if (this.Type == "SheetTextBox" && this.Editable) {
                if (this.DataField == "SYS_XM" || this.DataField == "SYS_IDCARD") {
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        var i = $("<i class=\"icon ion-camera\" style=\"right:11px;position:absolute;top:0px;padding:12px\"></i>");
                        i.unbind("click.camera").bind("click.camera", function () { open_idcard_diag_ysp(this) });

                        $(this.Element).after(i);
                    }
                    else {
                        var i = $("<a class=\"glyphicon glyphicon-camera\" data-toggle=\"modal\" data-target=\"#modal_idcard\" style=\"right:20px;position:absolute;top:0px;padding:12px\"></a>");
                        i.unbind("click.camera").bind("click.camera", function () { set_idcard_index(this) });
                        $(this.Element).after(i);
                    }
                }
            }
        };
        
        function validateIdentifyCode(con) {
            return IdentityCodeValid($(con).val());
        }

        // 表单验证接口
        $.MvcSheet.Validate = function () {

            var cardNumber = $.MvcSheetUI.GetElement("SYS_BANKCARDNUMBER").val();
            var cardNumber2 = $.MvcSheetUI.GetElement("SYS_BANKCARDNUMBER2").val();

            if (cardNumber.length < 15) {
                shakeMsg("银行卡号最少15位");
                return false;

            }

            if (cardNumber2 != "" && cardNumber2 != null && cardNumber2 != "null" && cardNumber2 != undefined) {
                if (cardNumber2.length < 15) {
                    shakeMsg("银行卡号最少15位");
                    return false;
                }
            }
            

            // 填写环节
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                return IdentityCodeValid($.MvcSheetUI.GetElement("SYS_IDCARD").val());
            }
        }

        //页面加载的事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                $("#modal_idcard").remove();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                //不是经销商账号，不可以自动发起进件的流程
                if ($.MvcSheetUI.SheetInfo.OriginatorOU != "28a0ba01-4f58-4097-96cb-77c2b09e8253"
                    && $.MvcSheetUI.SheetInfo.OriginatorOU != "9b8eb0fb-31c2-4577-bea7-9e453812f863") {
                    $.MvcSheetUI.GetElement("AUTO_INITIATE").prop("checked", false).attr("disabled", "disabled");
                }
            }
            var v_msg = $.MvcSheetUI.SheetInfo.BizObject.DataItems.SYS_MESSAGE.V;
            var sysCode = $.MvcSheetUI.SheetInfo.BizObject.DataItems["SYS_CODE"].V;

            var showrule = false;

            if (sysCode == "07") {

                var sysmessage = "<div class=\"row\"><div  id=\"title87\" class=\"col-md-2\">驳回原因</div><div id=\"title88\" class=\"col-md-10\"><textarea style=\"color:red\" readonly>" + v_msg + "</textarea></div></div>";

                $("#ysp_result").after(sysmessage);

            }
            else if (v_msg && v_msg != "") {
                var h_html = "<div class=\"row\"><div class=\"col-md-12\"><h4>命中规则</h4>";
                if (v_msg == "黑名单拒绝") {
                    h_html += "<ul><li class=\"red\">" + v_msg + "</li></ul>";
                    showrule = true;
                }
                else {
                    h_html += "<ul>";
                    var json_msg = $.parseJSON(v_msg);
                    $(json_msg).each(function (n, v) {
                        if (v.description == "直接拒绝") {
                            h_html += "<li class=\"red\">[" + v.description + "]&nbsp;" + v.ruleName + "</li>";
                            showrule = true;
                        }
                        //else if (v.description == "提示风险") {
                        //    h_html += "<li class=\"yellow\">[" + v.description + "]&nbsp;" + v.ruleName + "</li>";
                        //}
                        //else {
                        //    h_html += "<li>[" + v.description + "]&nbsp;&nbsp;&nbsp;" + v.ruleName + "</li>";
                        //}
                    });
                    h_html += "</ul>";
                }
                h_html += "</div></div>";
                if (showrule) {
                    $("#ysp_result").after(h_html);
                }
            }

            
            

        }
    </script>
    <style type="text/css">
        li {
            margin-left: 18px;
            line-height: 20px;
        }
            li.red {
                color: red;
            }
            li.yellow {
                color: #d4d40f;
            }
    </style>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">预审批流程</label>
    </div>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-right bannerTitle">
            <label id="divBasicInfo" data-en_us="Basic information">四要素</label>
        </div>
        <div class="divContent">
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="SYS_XM" style="">主借人姓名</span>
                </div>
                <div id="control5" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="SYS_XM" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title6" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="SYS_IDCARD" style="">主借人身份证号</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control14" type="text" data-datafield="SYS_IDCARD" data-type="SheetTextBox" style="" class=""
                        data-onchange="validateIdentifyCode(this)" maxlength="18">
                </div>
            </div>
            <div class="row">
                <div id="title8" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="SYS_PHONE" style="">主借人手机</span>
                </div>
                <div id="control8" class="col-md-4">
                    <input id="Control18" type="text" data-datafield="SYS_PHONE" data-type="SheetTextBox" style="" class=""
                        data-regularexpression="/^[1][3,4,5,6,7,8,9][0-9]{9}$/" data-regularinvalidtext="请输入正确的手机号码" maxlength="11">
                </div>
                <div id="title10" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="SYS_BANKCARDNUMBER" style="">主借人银行卡号</span>
                </div>
                <div id="control10" class="col-md-4">
                    <input id="Control16" type="text" oninput="textnum(this)"  maxlength="20" data-datafield="SYS_BANKCARDNUMBER" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title18" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="SYS_BANKCARDNUMBER2" style="">主借人银行卡号二</span>
                </div>
                <div id="control18" class="col-md-4">
                    <input id="Control17" type="text" oninput="textnum(this)"  maxlength="20" data-datafield="SYS_BANKCARDNUMBER2" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title19" class="col-md-2">
                    
                </div>
                <div id="control19" class="col-md-4">
                    
                </div>
            </div>
            <div class="row" id="ysp_result">
                <div id="title43" class="col-md-2">
					<span id="Label1423" data-type="SheetLabel" data-datafield="AUTO_INITIATE" style="">通过后是否自动发起零售流程</span>
				</div>
				<div id="control423" class="col-md-4">
					<input id="Control1423" type="checkbox" data-datafield="AUTO_INITIATE" data-type="SheetCheckbox" class="" style="" data-text="自动发起" data-defaultvalue="true">
				</div>
                <div id="title4" class="col-md-2">
					<span id="Label142" data-type="SheetLabel" data-datafield="SYS_PASS" style="">预审批结果</span>
				</div>
				<div id="control42" class="col-md-4">
					<input id="Control142" type="checkbox" data-datafield="SYS_PASS" data-type="SheetCheckbox" class="" style="" data-text="通过">
				</div>
            </div>
            <div class="row">
                <div id="title" class="col-md-12">
                    <h4>说明：</h4>
                    <ul>
                        <li>1.只需要输入主借人的四要素；</li>
                        <li>2.预审批结果可在已完成任务中查看；</li>
                        <li>3.勾选自动发起通过后系统自动发起进件流程，可在待办任务中查看；</li>
                        <li>4.预审批系统不通过则不允许进件，且以后都不行；（<span style="color:red">强控</span>）</li>
                        <li>5.复制单据时，如果姓名或身份证号发生修改则必须要走预审批流程，否则无法提交；</li>
                        <li>6.如有问题请联系管理员；</li>
                    </ul>
                </div>
            </div>
        </div>

        
    </div>

    <div class="modal fade" id="modal_idcard" tabindex="-1" role="dialog" aria-labelledby="modal_idcard_title" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                &times;
                            </button>
                            <h4 class="modal-title" id="modal_idcard_title">身份证识别</h4>
                        </div>
                        <div class="modal-body">
                            <div class="hidden">
                                <input type="text" id="hide_rowindex_field"/>
                            </div>
                            <div class="container" style="width: 100%;margin:10px 0">
                                <div class="row">
                                    <div class="col-md-12">
                                        <label>身份证（正面）</label>
                                        <a class="btn" style="float: right" onclick="idcard_reupload('0')">
                                            <i class="glyphicon glyphicon-repeat">重新选择</i>
                                        </a>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12" style="text-align:center">
                                        <input id="idcard_file_0" type="file" style="width: 100%" onchange="idcard_identify(this,'0')">
                                        <img id="idcard_img_0" style="width: auto; max-width: 100%; display: none;max-height:250px" src="" />
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-12">
                                        <label>身份证（反面）</label>
                                        <a class="btn" style="float: right" onclick="idcard_reupload('1')">
                                            <i class="glyphicon glyphicon-repeat">重新选择</i>
                                        </a>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-12" style="text-align:center">
                                        <input id="idcard_file_1" type="file" style="width: 100%" onchange="idcard_identify(this,'1')">
                                        <img id="idcard_img_1" style="width: auto; max-width: 100%; display: none;max-height:250px" src="" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5">
                                        <label>姓名</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_0_name"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5">
                                        <label>身份证号</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_0_idNumber"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5">
                                        <label>出生日期</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_0_birthday"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5">
                                        <label>性别</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_0_sex"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5">
                                        <label>民族</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_0_people"></label>
                                    </div>
                                </div>
                                <div class="row bottom">
                                    <div class="col-md-5">
                                        <label>住址</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_0_address"></label>
                                    </div>
                                </div>
                                <div class="row hidden">
                                    <div class="col-md-5">
                                        <label>签发机关</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_1_issueAuthority"></label>
                                    </div>
                                </div>
                                <div class="row bottom hidden">
                                    <div class="col-md-5">
                                        <label>有效期</label>
                                    </div>
                                    <div class="col-md-7">
                                        <label id="idcard_1_validity"></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">
                                取消
                            </button>
                            <a class="btn btn-primary" onclick="idcard_confirm_ysp()">确认
                            </a>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal -->
            </div>
</asp:Content>
