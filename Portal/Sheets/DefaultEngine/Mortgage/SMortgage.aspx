<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SMortgage.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SMortgage.SMortgage" EnableEventValidation="false" MasterPageFile="~/MvcSheetMortgage.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <%--<div style="text-align: center;" class="DragContainer">
	<label id="lblTitle" class="panel-title">抵押流程</label>
</div>--%>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-right bannerTitle">
            <label id="divBasicInfo" data-en_us="Basic information" class="">基本信息</label>
        </div>
        <div class="divContent">
            <div class="row">
                <div id="divFullNameTitle" class="col-md-2">
                    <label id="lblFullNameTitle" data-type="SheetLabel" data-datafield="Originator.UserName" data-en_us="Originator" data-bindtype="OnlyVisiable" style="">发起人</label>
                </div>
                <div id="divFullName" class="col-md-4">
                    <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style=""></label>
                </div>
                <div id="divOriginateTimeTitle" class="col-md-2">
                    <label id="lblOriginateTimeTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                </div>
                <div id="divOriginateTime" class="col-md-4">
                    <label id="lblOriginateTime" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style=""></label>
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
            <div class="row">
                <div id="title1" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="province" style="">所在省份</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input id="Control11" type="text" data-datafield="province" data-type="SheetTextBox" class="" style="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="city" style="">所在城市</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="city" data-type="SheetTextBox" class="" style="">
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
        </div>
        <div class="divContent" id="divSheet">

            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="applyType" style="">申请类型</span>
                </div>
                <div id="control3" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="applyType" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title4" class="col-md-2">
                </div>
                <div id="control4" class="col-md-4">
                </div>
            </div>
            <div class="row tableContent">
                <div id="div750666" class="col-md-2">
                    <label data-datafield="GJPeople" data-type="SheetLabel" id="ctl93378" class="" style="">信息</label></div>
                <div id="div970496" class="col-md-10">
                    <table id="ctl295580" data-datafield="GJPeople" data-type="SheetGridView" class="SheetGridView" style="" data-displayadd="false" data-displaydelete="false">
                        <tbody>
                            <tr class="header">
                                <td id="ctl295580_SerialNo" class="rowSerialNo">序号</td>
                                <td id="ctl295580_header0" data-datafield="GJPeople.People" style="">
                                    <label id="ctl295580_Label0" data-datafield="GJPeople.People" data-type="SheetLabel" style="">姓名</label></td>
                                <td id="ctl295580_header1" data-datafield="GJPeople.ID" style="">
                                    <label id="ctl295580_Label1" data-datafield="GJPeople.ID" data-type="SheetLabel" style="">证件号</label></td>
                                <td id="ctl295580_header2" data-datafield="GJPeople.IdType" style="">
                                    <label id="ctl295580_Label2" data-datafield="GJPeople.IdType" data-type="SheetLabel" style="">证件类型</label></td>
                                <td id="ctl295580_header3" data-datafield="GJPeople.PeopleType" style="">
                                    <label id="ctl295580_Label3" data-datafield="GJPeople.PeopleType" data-type="SheetLabel" style="">人员类型</label></td>
                                <td class="rowOption">删除</td>
                            </tr>
                            <tr class="template">
                                <td class=""></td>
                                <td id="ctl295580_control0" data-datafield="GJPeople.People" style="" class="">
                                    <input type="text" data-datafield="GJPeople.People" data-type="SheetTextBox" id="ctl295580_control0" style="" class=""></td>
                                <td id="ctl295580_control1" data-datafield="GJPeople.ID" style="" class="">
                                    <input type="text" data-datafield="GJPeople.ID" data-type="SheetTextBox" id="ctl295580_control1" style="" class=""></td>
                                <td id="ctl295580_control2" data-datafield="GJPeople.IdType" style="">
                                    <input type="text" data-datafield="GJPeople.IdType" data-type="SheetTextBox" id="ctl295580_control2" style=""></td>
                                <td id="ctl295580_control3" data-datafield="GJPeople.PeopleType" style="">
                                    <input type="text" data-datafield="GJPeople.PeopleType" data-type="SheetTextBox" id="ctl295580_control3" style="" class=""></td>
                                <td class="rowOption"><a class="delete">
                                    <div class="fa fa-minus"></div>
                                </a><a class="insert">
                                    <div class="fa fa-arrow-down"></div>
                                </a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>


            <div class="row">
                <div id="title9" class="col-md-2">

                    <span id="Label20" data-type="SheetLabel" data-datafield="assets" style="" class="">资产状况</span>
                </div>
                <div id="control9" class="col-md-4">

                    <input id="Control20" type="text" data-datafield="assets" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title10" class="col-md-2">
                </div>
                <div id="control10" class="col-md-4">
                </div>
            </div>
            <div class="row">
                <div id="title11" class="col-md-2">
                    <span id="Label21" data-type="SheetLabel" data-datafield="factory" style="">制造商</span>
                </div>
                <div id="control11" class="col-md-4">
                    <input id="Control21" type="text" data-datafield="factory" data-type="SheetTextBox" style="">
                </div>
                <div id="title12" class="col-md-2">
                    <span id="Label22" data-type="SheetLabel" data-datafield="carType" style="">车型</span>
                </div>
                <div id="control12" class="col-md-4">
                    <input id="Control22" type="text" data-datafield="carType" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title13" class="col-md-2">

                    <label data-datafield="newCarPrice" data-type="SheetLabel" id="ctl684477" class="" style="">新车指导价</label>
                </div>
                <div id="control13" class="col-md-4">

                    <input type="text" data-datafield="newCarPrice" data-type="SheetTextBox" id="ctl450208" class="" style="">
                </div>
                <div id="title14" class="col-md-2">

                    <label data-datafield="assetsPrice" data-type="SheetLabel" id="ctl54326" class="" style="">资产价格</label>
                </div>
                <div id="control14" class="col-md-4">

                    <input type="text" data-datafield="assetsPrice" data-type="SheetTextBox" id="ctl203870" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="title15" class="col-md-2">
                    <span id="Label25" data-type="SheetLabel" data-datafield="motorType" style="">发动机号码</span>
                </div>
                <div id="control15" class="col-md-4">
                    <input id="Control25" type="text" data-datafield="motorType" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title16" class="col-md-2">
                    <span id="Label26" data-type="SheetLabel" data-datafield="frameNumber" style="">车架号</span>
                </div>
                <div id="control16" class="col-md-4">
                    <input id="Control26" type="text" data-datafield="frameNumber" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title17" class="col-md-2">
                    <span id="Label27" data-type="SheetLabel" data-datafield="productClass" style="">产品组</span>
                </div>
                <div id="control17" class="col-md-4">
                    <input id="Control27" type="text" data-datafield="productClass" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title18" class="col-md-2">
                    <span id="Label28" data-type="SheetLabel" data-datafield="productType" style="">产品类型</span>
                </div>
                <div id="control18" class="col-md-4">
                    <input id="Control28" type="text" data-datafield="productType" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title19" class="col-md-2">

                    <label data-datafield="SFMoney" data-type="SheetLabel" id="ctl613883" class="" style="">首付金额</label>
                </div>
                <div id="control19" class="col-md-4">

                    <input type="text" data-datafield="SFMoney" data-type="SheetTextBox" id="ctl358983" class="" style="">
                </div>
                <div id="title20" class="col-md-2">

                    <label data-datafield="DKMoney" data-type="SheetLabel" id="ctl599418" class="" style="">贷款金额</label>
                </div>
                <div id="control20" class="col-md-4">

                    <input type="text" data-datafield="DKMoney" data-type="SheetTextBox" id="ctl868084" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="title21" class="col-md-2">

                    <label data-datafield="FJMoney" data-type="SheetLabel" id="ctl592649" class="" style="">附加费</label>
                </div>
                <div id="control21" class="col-md-4">

                    <input type="text" data-datafield="FJMoney" data-type="SheetTextBox" id="ctl861134" class="" style="">
                </div>
                <div id="title22" class="col-md-2">
                </div>
                <div id="control22" class="col-md-4">
                </div>
            </div>
            <div class="row">
                <div id="title445" class="col-md-12">
                    <span id="Label398" data-type="SheetLabel" data-datafield="LY" style="">留言信息</span>
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
                                <td style="width: 120px;">
                                    <label>留言人员</label>
                                </td>
                                <td style="width: 135px;">
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
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">发票</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row">
                    <div id="div630436" class="col-md-2">
                        <span id="Label32" data-type="SheetLabel" data-datafield="isSelf" style="" class="">是否自办</span>
                    </div>
                    <div id="div457705" class="col-md-2">
                        <div data-datafield="isSelf" data-type="SheetRadioButtonList" id="ctl462752" class="" style="" data-defaultitems="是;否" data-defaultselected="true"  data-repeatcolumns="2" data-onchange="if($.MvcSheetUI.GetControlValue(&quot;isSelf&quot;)==&quot;是&quot;){$(&quot;[data-datafield=&#39;SelfRun&#39;]&quot;).prev().show();} else {$(&quot;[data-datafield=&#39;SelfRun&#39;]&quot;).prev().hide();} if($.MvcSheetUI.GetControlValue(&quot;SelfRun&quot;)==&quot;自办上牌及抵押&quot;&amp;&amp;$.MvcSheetUI.GetControlValue(&quot;isSelf&quot;)==&quot;是&quot;){$(&quot;[data-datafield=&#39;selectShop&#39;]&quot;).hide().prev().hide();} else {$(&quot;[data-datafield=&#39;selectShop&#39;]&quot;).show().prev().show();} $.MvcSheetUI.GetElement(&quot;SPCity&quot;).change();"></div>
                    </div>
                    <div id="div457706" class="col-md-2">
                        <select data-datafield="SelfRun" data-type="SheetDropDownList" id="ctl888251" class="" style="" data-defaultitems="自办上牌;自办上牌及抵押" data-onchange="if($.MvcSheetUI.GetControlValue(&quot;SelfRun&quot;)==&quot;自办上牌及抵押&quot;&amp;&amp;$.MvcSheetUI.GetControlValue(&quot;isSelf&quot;)==&quot;是&quot;){$(&quot;[data-datafield=&#39;selectShop&#39;]&quot;).hide().prev().hide();} else {$(&quot;[data-datafield=&#39;selectShop&#39;]&quot;).show().prev().show();} $.MvcSheetUI.GetElement(&quot;SPCity&quot;).change();"></select>
                    </div>
                    <div id="div457707" class="col-md-2">
                        <input type="text" data-datafield="localDo" data-type="SheetTextBox" id="ctl502637" class="" style="" disabled="disabled" >
                        <%--<select data-datafield="localDo" data-type="SheetDropDownList" id="ctl888252" class="" style="" data-defaultitems="本地;异地"></select>--%>
                    </div>
                    <%--data-onchange="var state=$.MvcSheetUI.GetControlValue(&quot;SelfRun&quot;);if(state==&quot;自办抵押&quot;){$(&quot;[data-datafield=&#39;localDo&#39;]&quot;).prev().hide();} else {$(&quot;[data-datafield=&#39;localDo&#39;]&quot;).prev().show();} "
                data-onchange="var state=$.MvcSheetUI.GetControlValue(&quot;isSelf&quot;);if(state==&quot;是&quot;){$(&quot;[data-datafield=&#39;SelfRun&#39;]&quot;).prev().show();$(&quot;[data-datafield=&#39;localDo&#39;]&quot;).prev().show();} else {$(&quot;[data-datafield=&#39;SelfRun&#39;]&quot;).prev().hide();$(&quot;[data-datafield=&#39;localDo&#39;]&quot;).prev().hide();} "
                    --%>
                    <%--<div id="div772766" class="col-md-4">
				    <div class="row">
				    <div id="div772766" class="col-md-6">
				        <div data-datafield="isSelf" data-type="SheetRadioButtonList" id="ctl171783" class="" style="" data-defaultitems="是;否" data-repeatcolumns="2" data-onchange="var state=$.MvcSheetUI.GetControlValue(&quot;isSelf&quot;);
                                    if(state==&quot;是&quot;){
                                        $(&quot;#s2id_ctl888250&quot;).show();
                                        //$(&quot;#s2id_ctl888250555&quot;).show();  
                                    } else {
                                        $(&quot;#s2id_ctl888250&quot;).show();
                                        //$(&quot;#s2id_ctl888250555&quot;).hide();
                                    }
                                    ">
				        </div>
			        </div>
					    <div class="col-md-6">						
					        <select data-datafield="localDo" data-type="SheetDropDownList" id="ctl791266" class="" style="" data-defaultitems="本地;异地"></select>
                        </div>
				    </div>
			    </div>--%>
                    <div id="div196533" class="col-md-2">
                        <span id="Label34" data-type="SheetLabel" data-datafield="selectShop" style="" class="">办理店选择</span>
                    </div>
                    <div id="divSelectShop" class="col-md-2" style="display: none;">
                        <select data-type="SheetDropDownList" id="ctl888250" class="" data-defaultitems="" onchange="$.MvcSheetUI.SetControlValue(&quot;selectShop&quot;, $(&quot;#divSelectShop&quot;).find(&quot;select&quot;).find(&quot;option:selected&quot;).text());$.MvcSheetUI.SetControlValue(&quot;DyName&quot;, $(&quot;#divSelectShop&quot;).find(&quot;select&quot;).find(&quot;option:selected&quot;).val().split(&quot;,&quot;)[0]);$.MvcSheetUI.SetControlValue(&quot;SpName&quot;, $(&quot;#divSelectShop&quot;).find(&quot;select&quot;).find(&quot;option:selected&quot;).val().split(&quot;,&quot;)[1]);" ></select>
                        <%--<select data-datafield="selectShop" data-type="SheetDropDownList" id="ctl888250" class="" style="" data-defaultitems="东正办理;东正外包;主办店办理" data-onchange="$.ajax({&#10;        type: &quot;Post&quot;,&#10;        url:&quot;/Portal/MortgageSearch/Index?codeid=&quot; + $.MvcSheetUI.GetControlValue(&quot;SPCity&quot;) + &quot;&amp;shop=&quot; + $.MvcSheetUI.GetControlValue(&quot;selectShop&quot;),&#10;        success: function(g) {$.MvcSheetUI.GetElement(&quot;SPCity&quot;).attr(&quot;data-alter&quot;,g);}&#10;    });"></select>--%>
                    </div>
                    <div id="div465145" class="col-md-2">
                        <input type="text" data-datafield="selectShop" data-type="SheetTextBox" id="ctl502638" class="" style="" disabled="disabled" >
                    </div>
                </div>
                <div class="row">
                    <div id="title25" class="col-md-2">
                        <span id="Label35" data-type="SheetLabel" data-datafield="SPProvince" style="" class="">上牌抵押省份</span>
                    </div>
                    <div id="control25" class="col-md-4">
                        <select data-datafield="SPProvince" data-type="SheetDropDownList" id="ctl572263" class="" style="" data-schemacode="area" data-querycode="area" data-datavaluefield="CODEID" data-datatextfield="CODENAME"></select>
                    </div>
                    <div id="title26" class="col-md-2">
                        <span id="Label36" data-type="SheetLabel" data-datafield="SPCity" style="">上牌抵押城市</span>
                    </div>
                    <div id="control26" class="col-md-4">
                        <select data-datafield="SPCity" data-type="SheetDropDownList" id="ctl544588" class="" style="" data-schemacode="area" data-querycode="area" data-filter="SPProvince:PARENTID" data-datavaluefield="CODEID" data-datatextfield="CODENAME" data-onchange="if ($.MvcSheetUI.GetControlValue(&quot;province&quot;).replace(&quot;省&quot;, &quot;&quot;).replace(&quot;市&quot;, &quot;&quot;).replace(&quot;自治区&quot;, &quot;&quot;) == $.MvcSheetUI.GetElement(&quot;SPProvince&quot;).find(&quot;option:selected&quot;).text().replace(&quot;省&quot;, &quot;&quot;).replace(&quot;市&quot;, &quot;&quot;).replace(&quot;自治区&quot;, &quot;&quot;) &amp;&amp; $.MvcSheetUI.GetControlValue(&quot;city&quot;).replace(&quot;市&quot;, &quot;&quot;) == $.MvcSheetUI.GetElement(&quot;SPCity&quot;).find(&quot;option:selected&quot;).text().replace(&quot;市&quot;, &quot;&quot;)) $.MvcSheetUI.SetControlValue(&quot;localDo&quot;, &quot;本地&quot;);else $.MvcSheetUI.SetControlValue(&quot;localDo&quot;, &quot;异地&quot;);
if($.MvcSheetUI.SheetInfo.ActivityCode == &quot;Activity2&quot;){$.ajax({type: &quot;Post&quot;,url:&quot;/Portal/MortgageSearch/Index?codeid=&quot; + $.MvcSheetUI.GetControlValue(&quot;SPCity&quot;) + &quot;&amp;shop=&quot; + $.MvcSheetUI.GetControlValue(&quot;selectShop&quot;) + &quot;&amp;provinceid=&quot; + $.MvcSheetUI.GetControlValue(&quot;SPProvince&quot;),success: function(g) {if (($.MvcSheetUI.GetControlValue(&quot;SelfRun&quot;) == &quot;自办上牌&quot; &amp;&amp; $.MvcSheetUI.GetControlValue(&quot;isSelf&quot;) == &quot;是&quot;) || $.MvcSheetUI.GetControlValue(&quot;isSelf&quot;) == &quot;否&quot;) {if (g.indexOf(&quot;|&quot;) < 0) {$(&quot;#divSelectShop&quot;).css(&quot;display&quot;, &quot;none&quot;).next().css(&quot;display&quot;, &quot;block&quot;);$.MvcSheetUI.SetControlValue(&quot;selectShop&quot;, g.split(&quot;:&quot;)[0]);$.MvcSheetUI.SetControlValue(&quot;SpName&quot;, g.split(&quot;:&quot;)[1].split(&quot;,&quot;)[0]);$.MvcSheetUI.SetControlValue(&quot;DyName&quot;, g.split(&quot;:&quot;)[1].split(&quot;,&quot;)[1]);} else {var option = &quot;&quot;;var list = g.split(&quot;|&quot;);for (var i = 0; i < list.length; i++) {option += &quot;<option value=\&quot;&quot; + list[i].split(&quot;:&quot;)[1] + &quot;\&quot;>&quot; + list[i].split(&quot;:&quot;)[0] + &quot;</option>&quot;;}$(&quot;#divSelectShop&quot;).find(&quot;select&quot;).html(option);$(&quot;#divSelectShop&quot;).css(&quot;display&quot;, &quot;block&quot;).next().css(&quot;display&quot;, &quot;none&quot;);$(&quot;#divSelectShop&quot;).find(&quot;select&quot;).change();}} else $(&quot;#divSelectShop&quot;).css(&quot;display&quot;, &quot;none&quot;).next().css(&quot;display&quot;, &quot;none&quot;);}});}"></select>
                    </div>
                </div>
                <div class="row" hidden="">
                    <div id="div562459" class="col-md-2">
                        <label data-datafield="DyName" data-type="SheetLabel" id="ctl225113" class="" style="">抵押员</label>
                    </div>
                    <div id="div202645" class="col-md-4">
                        <input type="text" data-datafield="DyName" data-type="SheetTextBox" id="ctl748509" class="" style="">
                    </div>
                    <div id="div97627" class="col-md-2">
                        <label data-datafield="SpName" data-type="SheetLabel" id="ctl551722" class="" style="">上牌员</label>
                    </div>
                    <div id="div364916" class="col-md-4">
                        <input type="text" data-datafield="SpName" data-type="SheetTextBox" id="ctl550519" class="" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title27" class="col-md-2">
                        <span id="Label37" data-type="SheetLabel" data-datafield="certificate" style="">发票</span>
                    </div>
                    <div id="control27" class="col-md-10">
                        <div id="Control37" data-datafield="certificate" data-type="SheetAttachment" style="" class="" data-fileextensions=".jpg,.png,.gif,.jpeg"></div>
                    </div>
                </div>
            </div>
            <div class="spxx">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">上牌信息</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title29" class="col-md-2">
                            <span id="Label38" data-type="SheetLabel" data-datafield="licenceNo" style="">牌照号</span>
                        </div>
                        <div id="control29" class="col-md-4">
                            <input id="Control38" type="text" data-datafield="licenceNo" data-type="SheetTextBox" style="">
                        </div>
                        <div id="space30" class="col-md-2">
                        </div>
                        <div id="spaceControl30" class="col-md-4">
                        </div>
                    </div>
                    <div class="row">
                        <div id="title31" class="col-md-2">
                            <span id="Label39" data-type="SheetLabel" data-datafield="otherFile" style="">其他附件</span>
                        </div>
                        <div id="control31" class="col-md-10">
                            <div id="Control39" data-datafield="otherFile" data-type="SheetAttachment" style=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="dyyxzl">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">抵押影像资料</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title33" class="col-md-2">
                            <span id="Label40" data-type="SheetLabel" data-datafield="mortgageFile" style="">抵押资料</span>
                        </div>
                        <div id="control33" class="col-md-10">
                            <div id="Control40" data-datafield="mortgageFile" data-type="SheetAttachment" style="" class=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="dyspyj">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="div1SheetInfo" data-en_us="Sheet information">抵押审核意见</label>
                </div>
                <div class="divContent" id="divSheet0">
                    <div class="row">
                        <div id="title350" class="col-md-2">
                            <span id="Label410" data-type="SheetLabel" data-datafield="dyType" style="">审批</span>
                        </div>
                        <div id="control350" class="col-md-4">

                            <select data-datafield="dyType" data-type="SheetDropDownList" id="ctl2948460" class="" style="" data-defaultitems="同意;不同意"></select>
                        </div>
                        <div id="space360" class="col-md-2">
                        </div>
                        <div id="spaceControl360" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title370" class="col-md-2">
                            <span id="Label420" data-type="SheetLabel" data-datafield="dyAdvise" style="">抵押审批意见</span>
                        </div>
                        <div id="control370" class="col-md-10">
                            <div id="Control420" data-datafield="dyAdvise" data-type="SheetComment" style="" class=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="yyspyj">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divYYSheetInfo" data-en_us="Sheet information">运营审批意见</label>
                </div>
                <div class="divContent" id="divYYSheet">
                    <div class="row">
                        <div id="title35" class="col-md-2">
                            <span id="Label41" data-type="SheetLabel" data-datafield="spType" style="">审批</span>
                        </div>
                        <div id="control35" class="col-md-4">

                            <div data-datafield="spType" data-type="SheetRadioButtonList" id="ctl294846" class="" style="" data-defaultitems="同意;驳回" data-onchange="var state=$.MvcSheetUI.GetControlValue(&quot;spType&quot;);
                    if(state==&quot;同意&quot;){
                       $(&apos;[data-action=&quot;Submit&quot;]&apos;).show();
                       $(&apos;[data-action=&quot;Reject&quot;]&apos;).hide();
                    }
                    else if(state==&quot;驳回&quot;) {
                       $(&apos;[data-action=&quot;Submit&quot;]&apos;).hide();
                       $(&apos;[data-action=&quot;Reject&quot;]&apos;).show();
                    }">
                            </div>
                        </div>
                        <div id="space36" class="col-md-2">
                        </div>
                        <div id="spaceControl36" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title37" class="col-md-2">
                            <span id="Label42" data-type="SheetLabel" data-datafield="spAdvise" style="">审批意见</span>
                        </div>
                        <div id="control37" class="col-md-10">
                            <div id="Control42" data-datafield="spAdvise" data-type="SheetComment" style="" class=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="kddxx">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">快递单信息</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title39" class="col-md-2">
                            <span id="Label43" data-type="SheetLabel" data-datafield="expressNumber" style="" class="">快递单号</span>
                        </div>
                        <div id="control39" class="col-md-4">
                            <input id="Control43" type="text" data-datafield="expressNumber" data-type="SheetTextBox" style="" class="">
                        </div>
                        <div id="title40" class="col-md-2">
                        </div>
                        <div id="control40" class="col-md-4">
                        </div>
                    </div>
                </div>
            </div>
            <%--<div class="yysp">
			<div class="nav-icon fa  fa-chevron-right bannerTitle">
			<label id="divSheetInfo" data-en_us="Sheet information">运营审批意见</label>
		</div>
		<div class="divContent" id="divSheet">
			<div id="div315791" class="col-md-2"><span id="Label44" data-type="SheetLabel" data-datafield="rchivingResults" style="" class="">归档结果</span></div><div id="div171311" class="col-md-4"><input id="Control44" type="text" data-datafield="rchivingResults" data-type="SheetTextBox" style="" class=""></div><div id="div467684" class="col-md-2"></div><div id="div15195" class="col-md-4"></div></div>
			</div>--%>
        </div>

        <script type="text/javascript">

            function shakeMsg(msg) {
                if ($.MvcSheetUI.SheetInfo.IsMobile) {
                    alert(msg);
                } else {
                    if (typeof (layer) == "undefined") {
                        alert(msg);
                    } else {
                        layer.msg(msg, function () { });
                    }
                }
            }

            function showMsg(msg) {
                if ($.MvcSheetUI.SheetInfo.IsMobile) {
                    alert(msg);
                } else {
                    layer.msg(msg);
                }
            }


            function getDownLoadURL() {
                if ($(".SheetAttachment").find(".fa-download").length == 0) {
                    shakeMsg("附件为空！");
                } else {
                    window.open("Mortgage.html");
                }
                event.stopPropagation();
            }
            //添加留言
            function addmsg() {
                if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {
                    shakeMsg("请在保存或者提交后添加留言");
                    return false;
                }
                var userid = $.MvcSheetUI.SheetInfo.UserID;
                var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
                var msgval = $("#addmsg").val();
                if (!$("#addmsg").val() || $("#addmsg").val() == "") {
                    shakeMsg("请先填写信息！");
                    return false;
                }

                $.ajax({
                    //url: "/Portal/ajax/DZBizHandler.ashx",
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
            //获取留言信息
            function getmsg() {
                var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
                $.ajax({
                    //url: "/Portal/ajax/DZBizHandler.ashx",
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
                    //    shakeMsg(msg.responseText + "出错了");
                    //},
                    error: function (msg) {// 19.7 
					    showJqErr(msg);
                    }
                });
            }

            $.MvcSheet.Loaded = function () {
                if ($.MvcSheetUI.GetControlValue("SelfRun") == "自办上牌及抵押") $.MvcSheetUI.GetElement("selectShop").parent().hide().prev().hide();//$("[data-datafield='selectShop']").next().hide();
                if ($.MvcSheetUI.GetControlValue("isSelf") == "否") $("[data-datafield='SelfRun']").next().hide();
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                    $(".spxx,.dyyxzl,.dyspyj, .yyspyj,.kddxx").hide();

                    $('[data-action="Submit"]').click(function () {
                        var url = _PORTALROOT_GLOBAL + "/index.html#/app/Workflow/MyUnfinishedWorkItem";
                        window.opener.location.href = url;
                    });

                    $.MvcSheet.Submit = function (actionControl, text, destActivity, postValue, groupValue) {
                        if ($.MvcSheetUI.SheetInfo.IsMobile) {
                            var controls = $("#divSheet input[data-type='SheetTextBox']");
                            controls.each(function () {
                                $(this).trigger("change");
                            });
                        }
                        if (!$.MvcSheet.ActionValidata(actionControl)) return false;
                        if (($.MvcSheetUI.GetControlValue("isSelf") == "是" && $.MvcSheetUI.GetControlValue("SelfRun") == "自办上牌") || $.MvcSheetUI.GetControlValue("isSelf") == "否") {
                            if ($.MvcSheetUI.GetControlValue("selectShop") == "") {
                                alert("未匹配到正确的抵押规则，请联系管理员！");
                                return false;
                            }
                            if ($.MvcSheetUI.GetControlValue("SpName") == "" || $.MvcSheetUI.GetControlValue("DyName") == "") {
                                alert("未匹配到正确的抵押规则，请联系管理员！");
                                return false;
                            }
                        }
                        var that = this;
                        $.MvcSheet.ConfirmAction(SheetLanguages.Current.ConfirmDo + "【" + text + "】" + SheetLanguages.Current.Operation + "?",function() {
                            $.LoadingMask.Show(SheetLanguages.Current.Sumiting);
                            //增加一个提交并确认后的事件，此方法在许多需要与其它系统做集成的时候非常有用；Add by chenghs 2018-07-12
                            var callResult = true;
                            if ($.isFunction(that.AfterConfirmSubmit)) { //javascript函数
                                callResult = that.AfterConfirmSubmit.apply(that);
                            } else if (that.AfterConfirmSubmit) { //javascript语句
                                callResult = new Function(that.AfterConfirmSubmit).apply(that);
                            }
                            if (callResult) {
                                var SheetPostValue = that.GetMvcPostValue(that.Action_Submit, destActivity, postValue);
                                that.PostSheet(
                                    {
                                        Command: that.Action_Submit,
                                        MvcPostValue: JSON.stringify(SheetPostValue)
                                    },
                                    function(data) {
                                        that.ResultHandler.apply(that, [actionControl, data]);
                                    });
                            } else {
                                $.LoadingMask.Hide();
                            }
                        });
                    }
                }
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                    $(".dyyxzl,.dyspyj, .yyspyj,.kddxx").hide()
                }
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity9") {
                    var strdy = $.MvcSheetUI.SheetInfo.BizObject.DataItems["dyType"].V;
                    if (strdy == "不同意") {
                        $(".yyspyj, .kddxx").hide();
                    }
                    else {
                        $(".dyspyj, .yyspyj,.kddxx").hide();
                    }

                    var str = $.MvcSheetUI.SheetInfo.BizObject.DataItems["spType"].V;
                    if (str == "驳回") {
                        $(".dyspyj, .kddxx").hide();
                    }
                    else {
                        $(".dyspyj, .yyspyj,.kddxx").hide();
                    }
                }
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity37") {
                    $("#DownloadURL").show();
                    var str = $.MvcSheetUI.SheetInfo.BizObject.DataItems["spType"].V;
                    if (str == "驳回") {
                        $(".kddxx").hide();
                    }
                    else {
                        $(".yyspyj,.kddxx").hide();
                    }
                }
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity11") {
                    $(".kddxx").hide();
                    $("#DownloadURL").show();

                    var BizObjectId = $.MvcSheetUI.SheetInfo.BizObjectID;
                    $('[data-action="Submit"]').click(function () {

                        $.ajax({
                            url: "/Portal/MortgageAttachment/Index",//wangxg 19.7
                            //url: "/Portal/ajax/MortgageAttachment.ashx",
                            data: { 'BizObjectId': BizObjectId },
                            type: "post",
                            async: true,
                            dataType: "json",
                            success: function (result) {

                            },
                            error: function (msg) {// 19.7 
					             showJqErr(msg);
                            }
                        })
                    });
                }
                //获取留言
                getmsg();
                //添加留言
                $('#addmsga').on('click', function () { addmsg(); });
                //$("[data-datafield='SelfRun']").change(function () {
                //    alert($(this).val())
                //    if ($(this).val() == "自办抵押") $(this).parent().next().css("display", "none");
                //    else $(this).parent().next().css("display", "block");
                //});
                //if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity13") {
                //    $(".yysp").hide();
                //}           
            }
        </script>
    </div>
</asp:Content>
