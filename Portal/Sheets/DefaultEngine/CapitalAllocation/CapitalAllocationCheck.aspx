<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CapitalAllocationCheck.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.CapitalAllocationCheck" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <style>
        .test { }
    </style>
    <link href="<%=ResolveUrl("~/WFRes/layer/theme/default/layer.css")%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" charset="utf-8" src="<%=ResolveUrl("~/WFRes/layer/layer.js") %>"> </script>
<script type="text/javascript">
    $(function () {        
        $.MvcSheet.PreInit = function () {            //$.MvcSheetUI.SheetInfo.PermittedActions.Save = true;            //$.MvcSheetUI.SheetInfo.PermittedActions.ViewInstance = true;            //$.MvcSheetUI.SheetInfo.PermittedActions.Submit = true;            //$.MvcSheetUI.SheetInfo.PermittedActions.Print = true;            //$.MvcSheetUI.SheetInfo.PermittedActions.Reject = true;            //$.MvcSheetUI.SheetInfo.PermittedActions.Forward = false;            //$.MvcSheetUI.SheetInfo.PermittedActions.Close = true;        };

        $.MvcSheet.AfterConfirmSubmit = function () {            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {                var auditResult = $.MvcSheetUI.GetControlValue("fh_result", 1);                if (auditResult == "拒绝") {                    var v = $.MvcSheetUI.MvcRuntime.executeService('DZBIZServiceNew', 'FinishInstance',                        {                            InsID: $.MvcSheetUI.SheetInfo.InstanceId                        }                    );                    console.log(v);                }            }            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {                var auditResult = $.MvcSheetUI.GetControlValue("fh_result2", 1);                if (auditResult == "拒绝") {                    var v = $.MvcSheetUI.MvcRuntime.executeService('DZBIZServiceNew', 'FinishInstance',                        {                            InsID: $.MvcSheetUI.SheetInfo.InstanceId                        }                    );                    console.log(v);                }            }            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity17") {                var auditResult = $.MvcSheetUI.GetControlValue("manager_finc_result", 1);                if (auditResult == "拒绝") {                    var v = $.MvcSheetUI.MvcRuntime.executeService('DZBIZServiceNew', 'FinishInstance',                        {                            InsID: $.MvcSheetUI.SheetInfo.InstanceId                        }                    );                    console.log(v);                }            }            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity19") {                var auditResult = $.MvcSheetUI.GetControlValue("gen_manager_result", 1);                if (auditResult == "拒绝") {                    var v = $.MvcSheetUI.MvcRuntime.executeService('DZBIZServiceNew', 'FinishInstance',                        {                            InsID: $.MvcSheetUI.SheetInfo.InstanceId                        }                    );                    console.log(v);                }            }            return true;        };

        $('#showDetail').click(function () {
            var rowNo = this.id.replace('showDetail_Row','');
            var row = $.MvcSheetUI.GetControlValue("zjdb_table", 1)[rowNo - 1];
            console.log(row);
            var moneyType = "¥";
            var moneyTypeBig = "人民币 ";
            if (row.money_type !== '00001') {
                // todo 其他币种
            }
            $('.fk_date').text(row.fk_date);
            $('.fk_account').text(row.fk_account);
            $('.fk_bank').text(row.fk_bank);
            $('.fk_company').text(row.fk_company);
            $('.hk_money').text(moneyType + Number(row.hk_money).toLocaleString());
            $('.hk_money_big').text(moneyTypeBig + changeMoneyToChinese( row.hk_money));
            $('._note').text(row.note);
            $('.sk_account').text(row.sk_account);
            $('.sk_bank').text(row.sk_bank);
            $('.sk_user_label').text(row.sk_user_label);
            layer.open({
              type: 1,
              title: false,
              closeBtn: 1,
              area: '900px',
              skin: 'layui-layer-nobg', //没有背景色
              shadeClose: true,
              content: $('#divFormSingle')
            });
        });
        
        $('#printForm').click(function () {
            $('#divFormMuti').html('');
            var rows = $.MvcSheetUI.GetControlValue("zjdb_table", 1);
            for (var i = 0; i < rows.length; i++) {
                var tableClone = $('#singleTable').clone(true);
                
                var row = rows[i];
                var moneyType = "¥";
                var moneyTypeBig = "人民币 ";
                if (row.money_type !== '00001') {
                    // todo 其他币种
                }
                tableClone.find('.fk_date').text(row.fk_date);
                tableClone.find('.fk_account').text(row.fk_account);
                tableClone.find('.fk_bank').text(row.fk_bank);
                tableClone.find('.fk_company').text(row.fk_company);
                tableClone.find('.hk_money').text(moneyType + Number(row.hk_money).toLocaleString());
                console.log(Number(row.hk_money).toLocaleString());
                tableClone.find('.hk_money_big').text(moneyTypeBig + changeMoneyToChinese( row.hk_money));
                tableClone.find('._note').text(row.note);
                tableClone.find('.sk_account').text(row.sk_account);
                tableClone.find('.sk_bank').text(row.sk_bank);
                tableClone.find('.sk_user_label').text(row.sk_user_label);
          
                $('#divFormMuti').append(tableClone);
            }

            var wind = window.open("", 'newwindow', 'height=700, width=1000, top=100, left=100, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=n o, status=no');
            
            var style = wind.document.createElement("style");  
            style.type = "text/css";  
            style.appendChild(wind.document.createTextNode(
                "table,table tr th, table tr td { border:1px solid #666666;border-collapse: collapse; }  \
table {width: 100%;margin-bottom: 60px; }"));
            wind.document.getElementsByTagName("head")[0].appendChild(style);
            
            wind.document.body.innerHTML = $('#divFormMuti').html();
            wind.print();
        });
        
        function changeMoneyToChinese(money){  
            var cnNums = new Array("零","壹","贰","叁","肆","伍","陆","柒","捌","玖"); //汉字的数字  
            var cnIntRadice = new Array("","拾","佰","仟"); //基本单位  
            var cnIntUnits = new Array("","万","亿","兆"); //对应整数部分扩展单位  
            var cnDecUnits = new Array("角","分","毫","厘"); //对应小数部分单位  
            //var cnInteger = "整"; //整数金额时后面跟的字符  
            var cnIntLast = "元"; //整型完以后的单位  
            var maxNum = 999999999999999.9999; //最大处理的数字  
              
            var IntegerNum; //金额整数部分  
            var DecimalNum; //金额小数部分  
            var ChineseStr=""; //输出的中文金额字符串  
            var parts; //分离金额后用的数组，预定义  
            if( money == "" ){  
                return "";  
            }  
            money = parseFloat(money);  
            if( money >= maxNum ){  
                $.alert('超出最大处理数字');  
                return "";  
            }  
            if( money == 0 ){  
                //ChineseStr = cnNums[0]+cnIntLast+cnInteger;  
                ChineseStr = cnNums[0]+cnIntLast  
                //document.getElementById("show").value=ChineseStr;  
                return ChineseStr;  
            }  
            money = money.toString(); //转换为字符串  
            if( money.indexOf(".") == -1 ){  
                IntegerNum = money;  
                DecimalNum = '';  
            }else{  
                parts = money.split(".");  
                IntegerNum = parts[0];  
                DecimalNum = parts[1].substr(0,4);  
            }  
            if( parseInt(IntegerNum,10) > 0 ){//获取整型部分转换  
                zeroCount = 0;  
                IntLen = IntegerNum.length;  
                for( i=0;i<IntLen;i++ ){  
                    n = IntegerNum.substr(i,1);  
                    p = IntLen - i - 1;  
                    q = p / 4;  
                    m = p % 4;  
                    if( n == "0" ){  
                        zeroCount++;  
                    }else{  
                        if( zeroCount > 0 ){  
                            ChineseStr += cnNums[0];  
                        }  
                        zeroCount = 0; //归零  
                        ChineseStr += cnNums[parseInt(n)]+cnIntRadice[m];  
                    }  
                    if( m==0 && zeroCount<4 ){  
                        ChineseStr += cnIntUnits[q];  
                    }  
                }  
                ChineseStr += cnIntLast;  
                //整型部分处理完毕  
            }  
            if( DecimalNum!= '' ){//小数部分  
                decLen = DecimalNum.length;  
                for( i=0; i<decLen; i++ ){  
                    n = DecimalNum.substr(i,1);  
                    if( n != '0' ){  
                        ChineseStr += cnNums[Number(n)]+cnDecUnits[i];  
                    }  
                }  
            }  
            if( ChineseStr == '' ){  
                //ChineseStr += cnNums[0]+cnIntLast+cnInteger;  
                ChineseStr += cnNums[0]+cnIntLast;  
            }/* else if( DecimalNum == '' ){ 
                ChineseStr += cnInteger; 
                ChineseStr += cnInteger; 
            } */  
            return ChineseStr;  
        };

    })
</script>
    <style>
        .form-row{white-space:nowrap;}
    </style>
<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">资金调拨</label>
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
		<div class="divContent" id="divSheet">
			<div class="row">
				<%--<div id="title1" class="col-md-2">
					<span id="Label11" data-type="SheetLabel" data-datafield="hkxz" style="">划款性质</span>
				</div>
				<div id="control1" class="col-md-4">
					<select data-datafield="hkxz" data-type="SheetDropDownList" id="ctl163766" class="" style="" data-masterdatacategory="划款性质">
					</select>
				</div>--%>
				<div id="title2" class="col-md-2">
					<span id="Label12" data-type="SheetLabel" data-datafield="hkxz_note" style="">划款内容</span>
				</div>
				<div id="control2" class="col-md-10">
					<input id="Control12" type="text" data-datafield="hkxz_note" data-type="SheetTextBox" style="">
				</div>
			</div>
			<div class="row tableContent">
				<div id="title3" class="col-md-2">
					<span id="Label13" data-type="SheetLabel" data-datafield="zjdb_table" style="">划款表单</span>
                    <a href="#" id="printForm">打印</a>
				</div>
				<div id="control3" class="col-md-10" style="">
                    <div style="overflow: auto">
					<table id="Control13" data-datafield="zjdb_table" data-type="SheetGridView" class="SheetGridView"  style="min-width:600px">
						<tbody>
							
							<tr class="header">
								<td id="Control13_SerialNo" class="rowSerialNo" style="width: 40px;">
序号								</td>
								<td id="Control13_Header3" data-datafield="zjdb_table.fk_date" class="" style="width: 70px;display:none">
									<label id="Control13_Label3" data-datafield="zjdb_table.fk_date" data-type="SheetLabel" style="" class="">付款日期</label>
								</td>
								<td id="Control13_Header13" data-datafield="zjdb_table.hkxz" style="width: 110px;display:none">
									<label id="Control13_Label13" data-datafield="zjdb_table.hkxz" data-type="SheetLabel" style="">划款性质</label>
								</td>
								<td id="Control13_Header4" data-datafield="zjdb_table.fk_company" style="width: 110px;display:none">
									<label id="Control13_Label4" data-datafield="zjdb_table.fk_company" data-type="SheetLabel" style="">付款人户名</label>
								</td>
								<td id="Control13_Header5" data-datafield="zjdb_table.fk_bank" style="width: 110px;display:none">
									<label id="Control13_Label5" data-datafield="zjdb_table.fk_bank" data-type="SheetLabel" style="">付款人开户行</label>
								</td>
								<td id="Control13_Header6" data-datafield="zjdb_table.fk_account" style="width: 110px;display:none">
									<label id="Control13_Label6" data-datafield="zjdb_table.fk_account" data-type="SheetLabel" style="">付款人账号</label>
								</td>
								<td id="Control13_Header7" data-datafield="zjdb_table.sk_user_label" style="width: 110px;">
									<label id="Control13_Label7" data-datafield="zjdb_table.sk_user_label" data-type="SheetLabel" style="">收款人户名</label>
								</td>
								<td id="Control13_Header8" data-datafield="zjdb_table.sk_bank" style="width: 110px;display:none">
									<label id="Control13_Label8" data-datafield="zjdb_table.sk_bank" data-type="SheetLabel" style="">收款人开户行</label>
								</td>
								<td id="Control13_Header9" data-datafield="zjdb_table.sk_account" style="width: 110px;display:none">
									<label id="Control13_Label9" data-datafield="zjdb_table.sk_account" data-type="SheetLabel" style="" class="">收款人账号</label>
								</td>
								<td id="Control13_Header10" data-datafield="zjdb_table.money_type" style="width: 90px;display:none">
									<label id="Control13_Label10" data-datafield="zjdb_table.money_type" data-type="SheetLabel" style="width: 80px;" class="">币种</label>
								</td>
								<td id="Control13_Header11" data-datafield="zjdb_table.hk_money" style="width: 110px;">
									<label id="Control13_Label11" data-datafield="zjdb_table.hk_money" data-type="SheetLabel" style="">转账金额</label>
								</td>
								<td id="Control13_Header12" data-datafield="zjdb_table.note" style="width: 110px;display:none">
									<label id="Control13_Label12" data-datafield="zjdb_table.note" data-type="SheetLabel" style="">备注</label>
								</td>
                                <td>操作</td>
								<td class="rowOption">删除</td>
							</tr>
							<tr class="template">
								<td id="Control13_Option" class="rowOption">
								</td>
								<td data-datafield="zjdb_table.fk_date" style="display:none">
									<input type="text" data-datafield="zjdb_table.fk_date" data-type="SheetTime" id="ctl519857" class="" style="">
								</td>
								<td data-datafield="zjdb_table.hkxz" class="form-row" style="padding-right:10px;display:none">
									<select data-datafield="zjdb_table.hkxz" data-type="SheetDropDownList" id="ctl788059" class="" style="" data-masterdatacategory="划款性质">
									</select>
								</td>
								<td data-datafield="zjdb_table.fk_company" class="form-row" style="padding-right:10px;display:none">
									<input id="Control13_ctl4" type="text" data-datafield="zjdb_table.fk_company" data-type="SheetTextBox" style="" class="">
								</td>
								<td data-datafield="zjdb_table.fk_bank" class="form-row" style="padding-right:10px;display:none">
									<input id="Control13_ctl5" type="text" data-datafield="zjdb_table.fk_bank" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.fk_account" class="form-row" style="padding-right:10px;display:none">
									<input id="Control13_ctl6" type="text" data-datafield="zjdb_table.fk_account" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.sk_user_label" class="form-row" style="padding-right:10px">
									<input id="Control13_ctl7" type="text" data-datafield="zjdb_table.sk_user_label" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.sk_bank" class="form-row" style="padding-right:10px;display:none">
									<input id="Control13_ctl8" type="text" data-datafield="zjdb_table.sk_bank" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.sk_account" class="form-row" style="padding-right:10px;display:none">
									<input id="Control13_ctl9" type="text" data-datafield="zjdb_table.sk_account" data-type="SheetTextBox" style="">
								</td>
								<td data-datafield="zjdb_table.money_type" class="form-row" style="padding-right:10px;display:none">
									<select data-datafield="zjdb_table.money_type" data-type="SheetDropDownList" id="ctl521133" class="" style="" data-masterdatacategory="币别">
									</select>
								</td>
								<td data-datafield="zjdb_table.hk_money">
									<input id="Control13_ctl11" type="text" data-datafield="zjdb_table.hk_money" data-type="SheetTextBox" style="" class="" data-defaultvalue="0" data-formatrule="{0:C2}">
								</td>
								<td data-datafield="zjdb_table.note" style="display:none">
									<input id="Control13_ctl12" type="text" data-datafield="zjdb_table.note" data-type="SheetTextBox" style="">
								</td>
                                <td>
                                    <a href="#" id="showDetail">详细</a>
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
								<td data-datafield="zjdb_table.sk_user_label">
								</td>
								<td data-datafield="zjdb_table.hk_money" class="" style="text-align:left">
									<label id="Control13_stat11" data-datafield="zjdb_table.hk_money" data-type="SheetCountLabel" data-formatrule="{0:C2}" style="" class=""></label>
								</td>
								<td class="rowOption">
								</td>
								<td>
								</td>
							</tr>
						</tbody>
					</table>
                        </div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title5" class="col-md-2">
					<span id="Label14" data-type="SheetLabel" data-datafield="fh_remark" style="">复核意见</span>
				</div>
				<div id="control5" class="col-md-10">
					<div data-datafield="fh_result" data-type="SheetRadioButtonList" id="ctl116159" class="" style="" data-defaultitems="同意;拒绝" data-defaultselected="false"></div>
					<div id="Control14" data-datafield="fh_remark" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title7" class="col-md-2">
					<span id="Label15" data-type="SheetLabel" data-datafield="fh_remark2" style="">资金部总经理意见</span>
				</div>
				<div id="control7" class="col-md-10">
					<div data-datafield="fh_result2" data-type="SheetRadioButtonList" id="ctl396612" class="" style="" data-defaultitems="同意;拒绝" data-defaultselected="false"></div>
					<div id="Control15" data-datafield="fh_remark2" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title9" class="col-md-2">
					<span id="Label16" data-type="SheetLabel" data-datafield="manager_finc_remark" style="">分管资金副总意见</span>
				</div>
				<div id="control9" class="col-md-10">
					<div data-datafield="manager_finc_result" data-type="SheetRadioButtonList" id="ctl78716" class="" style="" data-defaultitems="同意;拒绝" data-defaultselected="false"></div>
					<div id="Control16" data-datafield="manager_finc_remark" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row tableContent">
				<div id="title11" class="col-md-2">
					<span id="Label17" data-type="SheetLabel" data-datafield="gen_manager_remark" style="">总经理意见</span>
				</div>
				<div id="control11" class="col-md-10">
					<div data-datafield="gen_manager_result" data-type="SheetRadioButtonList" id="ctl905254" class="" style="" data-defaultitems="同意;拒绝" data-defaultselected="false"></div>
					<div id="Control17" data-datafield="gen_manager_remark" data-type="SheetComment" style=""></div>
				</div>
			</div>
			<div class="row" style="display:none">
				<div id="title13" class="col-md-2">
					<span id="Label18" data-type="SheetLabel" data-datafield="pass_round" style="">传阅</span>
				</div>
				<div id="control13" class="col-md-4">
					<div id="Control18" data-datafield="pass_round" data-type="SheetUser" style=""></div>
				</div>
				<div id="space14" class="col-md-2">
				</div>
				<div id="spaceControl14" class="col-md-4">
				</div>
			</div>
			<div class="row">
				<div id="title15" class="col-md-2">
					<span id="Label19" data-type="SheetLabel" data-datafield="attachment" style="">附件</span>
				</div>
				<div id="control15" class="col-md-10">
					<div id="Control19" data-datafield="attachment" data-type="SheetAttachment" style=""></div>
				</div>
			</div>
		</div>
	</div>
<div style="display:none;padding:30px;background-color:white" id="divFormSingle">
                        <table class="table table-bordered table-condensed table-responsive" id="singleTable">
                            <tr>
                                <td colspan="6" style="text-align:center"><span class="" style="font-size:1.3em">资金内部划转申请表</span></td>
                            </tr>
                            <tr>
                                <td colspan="6" style="text-align:center"><span class="fk_date"></span></td>
                            </tr>
                            <tr>
                                <td rowspan="3" style="vertical-align:middle;padding:10px 20px; width: 20px; margin: 0 auto; line-height: 24px; ">付款人</td>
                                <td>全称</td>                                
                                <td style="min-width:200px"><span class="fk_company"></span></td>
                                <td rowspan="3" style="vertical-align:middle;padding:10px 20px; width: 20px; margin: 0 auto; line-height: 24px; ">收款人</td>
                                <td>全称</td>
                                <td style="min-width:200px"><span class="sk_user_label"></span></td>
                            </tr>
                            <tr>
                                <td>开户行</td>
                                <td><span class="fk_bank"></span></td>                                
                                <td>开户行</td>
                                <td><span class="sk_bank"></span></td>
                            </tr>
                            <tr>
                                <td>账号</td>
                                <td><span class="fk_account"></span></td>                                
                                <td>账号</td>
                                <td><span class="sk_account"></span></td>
                            </tr>
                            <tr>
                                <td rowspan="2" colspan="2" style="vertical-align:middle">币种及金额</td>
                                <td colspan="4"><span class="hk_money"></span></td>
                            </tr>
                            <tr>
                                <td colspan="4"><span class="hk_money_big"></span></td>
                            </tr>
                            <tr>
                                <td colspan="2">备注</td>
                                <td colspan="4"><span class="_note"></span></td>
                            </tr>
                            <tr>
                                <td colspan="2">付款日期</td>
                                <td colspan="4"><span class="fk_date"></span></td>
                            </tr>
                        </table>
</div>
        
<div style="display:none;padding:30px;background-color:white" id="divFormMuti">
</div>

</asp:Content>
