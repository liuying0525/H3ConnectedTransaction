<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dycs.aspx.cs"  Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.dycs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Pragma" content="text/html; charset=utf-8"/>
    <title></title>
    <%--<script src="js/jquery.min.js"></script>
    <link href="WFRes/assets/stylesheets/pixel-admin.min.css" rel="stylesheet" />
    <link href="WFRes/css/MvcSheetTest.css" rel="stylesheet" />--%>
     <script src="DZBiz/JsonExportExcel.min.js"  type="text/javascript"></script>
       <style type="text/css">
           table td{
                border:1px solid #ccc;
           }
    </style>
    <script type="text/javascript">
        // JavaScript Document
        $(document).ready(function (e) {
            debugger
            //向div里面仍三个下拉
            var str = "<select id='sheng'></select><select id='shi'></select><select id='qu'></select>";
            $("#sanjiliandong").html(str);//三个下拉显示
            $("#sanjiliandong").append("<a style=\" padding: 5px 12px;margin-left: 10px;\" data-dismiss=\"modal\" onclick=\"saveArea()\">添加</a>");//添加
            $("#sanjiliandong").append("<a style=\" padding: 5px 12px;margin-left: 10px;\" data-dismiss=\"modal\" onclick=\"searchArea()\">搜索</a>");//搜索

            //当省选中的话市也会跟着变去也会变。市和区都会加载一遍
            FillSheng();//省
            FillShi();//市
            FillQu();//区
            //给省加点击事件
            $("#sheng").change(function () {
                FillShi();//市
                FillQu();//区
            })
            //给市加点击事件
            $("#shi").change(function () {
                FillQu();//区
            })
        });
        $(document).ready(function (e) {
            getdycs();
           
        });
        
        //做三个方法分别为省市区
        function FillSheng() {
            var pcode = "100000";
            var UserCode = '<%=this.ActionContext.User.UserCode%>';
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/getssx",// 19.6.28 wangxg
                data: { CommandName: "getssx", pcode: pcode, UserCode: UserCode },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    var str = "<option value='' >请选择省份</option>";
                    //把行的数组遍历下用for循环...length长度
                    for (var i = 0; i < data.length; i++) {
                        //把行的索引i在拆下.列与列的分隔符再拆
                        str += "<option value='" + data[i].CODEID + "'>" + data[i].CODENAME + "</option>";
                        $("#sheng").html(str);
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //填充市的方法
        function FillShi() {
            var pcode = $("#sheng").val();
            $.ajax({
                async: false,//****
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/getssx",// 19.6.28 wangxg
                data: { CommandName: "getssx", pcode: pcode },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    var str = "<option value='' >请选择城市</option>";
                    //把行的数组遍历下用for循环...length长度
                    for (var i = 0; i < data.length; i++) {
                        str += "<option value='" + data[i].CODEID + "'>" + data[i].CODENAME + "</option>";
                    }
                    $("#shi").html(str);
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        //填充区的方法
        function FillQu() {
            var pcode = $("#shi").val();
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/getssx",// 19.6.28 wangxg
                data: { CommandName: "getssx", pcode: pcode },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    var str = "<option value='' >请选择县</option>";
                    //把行的数组遍历下用for循环...length长度
                    for (var i = 0; i < data.length; i++) {
                        str += "<option value='" + data[i].CODEID + "'>" + data[i].CODENAME + "</option>";
                    }
                    $("#qu").html(str);
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        
        ///保存抵押城市
        function saveArea() {
            var UserCode = '<%=this.ActionContext.User.UserCode%>';
            var shen="",shi="",xian="";
            shen = $("#sheng").find("option:selected").val();
            shi = $("#shi").find("option:selected").val();
            xian = $("#qu").find("option:selected").val();
            if (shen == ""){
                alert("省份不能为空！");
                return false;
            }
            if (shi == "") {
                alert("市不能为空！");
                return false;
            }
            if (xian == "") {
                alert("区/县不能为空！");
                return false;
            }
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/updatedycs",// 19.6.28 wangxg
                data: { CommandName: "updatedycs", UserCode: UserCode, shen: shen, shi: shi, xian: xian },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    alert("ok");
                    $("#shxTable").html("");
                    getdycs();
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });

        }
        function searchArea() {
            $("#shxTable").html("");
            debugger
            var UserCode = '<%=this.ActionContext.User.UserCode%>';
            shen = $("#sheng").find("option:selected").val();
            shi = $("#shi").find("option:selected").val();
            xian = $("#qu").find("option:selected").val();
            var ids = shen + "," + shi+ "," + xian;
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/searchdycs",// 19.6.28 wangxg
                data: { CommandName: "searchdycs", ids: ids, UserCode: UserCode },
                type: "POST",
                dataType: "json",
                success: function (result) {
                    for (var i = 0; i < result.length; i++) {
                        var tr = "<tr>";

                        tr += "<td>" + result[i].ROWNO + "</td>";
                        tr += "<td>" + result[i].SHEN + "</td>";
                        tr += "<td>" + result[i].SHI + "</td>";
                        tr += "<td>" + result[i].XIAN + "</td>";
                        tr += "<td><a style=\"color:blue\" onclick=\"del('" + result[i].SHENID + "," + result[i].SHIID + "," + result[i].XIANID + "')\">删除</a></td>";
                        tr += "</tr>";
                        $("#shxTable").append(tr);
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        function getdycs() {
            $("#shxTable").html("");
            var UserCode = '<%=this.ActionContext.User.UserCode%>';
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/getdycs",// 19.6.28 wangxg
                data: { CommandName: "getdycs", UserCode: UserCode },
                type: "POST",
                dataType: "json",
                success: function (result) {
                    for (var i = 0; i < result.length; i++) {
                        var tr = "<tr>";

                        tr += "<td>" + result[i].ROWNO + "</td>";
                        tr += "<td>" + result[i].SHEN + "</td>";
                        tr += "<td>" + result[i].SHI + "</td>";
                        tr += "<td>" + result[i].XIAN + "</td>";
                        tr += "<td><a  style=\"color:blue\" onclick=\"del('" + result[i].SHENID + "," + result[i].SHIID + "," + result[i].XIANID + "')\">删除</a></td>";
                        tr += "</tr>";
                        $("#shxTable").append(tr);
                    }
                },
                error: function (msg) {// 19.7 
                     showJqErr(msg);
                }
            });
         }
        function del(ids) {
            var UserCode = '<%=this.ActionContext.User.UserCode%>';
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/deldycs",// 19.6.28 wangxg
                data: { CommandName: "deldycs", ids: ids, UserCode: UserCode },
                type: "POST",
                dataType: "text",
                success: function (result) {
                    if (result == "1") {
                        alert("删除成功！");
                    }
                    else {
                        alert(result);
                    }
                    getdycs();
                },
                error: function (msg) {// 19.7 
                     showJqErr(msg);
                }
            });

        }
        function dowloadExcel() {
             var option={};
             option.fileName = "【"+CurentTime()+"】已抵押城市数据";
            var sheetData = "";
            debugger
            $.ajax({
                async: false,
                //url: "ajax/DZBizHandler.ashx",
                url: "/Portal/DZBizHandlerAuth/downloaddycs",// 19.6.28 wangxg
                data: { CommandName: "downloaddycs" },
                type: "POST",
                dataType: "json",
                success: function (result) {
                    sheetData = result;
                },
                error: function (msg) {// 19.7 
                     showJqErr(msg);
                }
            });
            option.datas=[
                {
                    sheetData: sheetData,
                    sheetName:'sheet',
                    //sheetFilter:['two','one'],
                    sheetHeader:['省','市','县'],

            }
            ];
            var toExcel=new ExportJsonExcel(option);
            toExcel.saveExcel();
        }
        function CurentTime() {
            var now = new Date();
            var year = now.getFullYear();       //年
            var month = now.getMonth() + 1;     //月
            var day = now.getDate();            //日
            var clock = year + "-";
            if (month < 10)
                clock += "0";
            clock += month + "-";
            if (day < 10)
                clock += "0";
            clock += day + " ";
            return (clock);
        }
    </script>

</head>
<body>
    <form id="form1" runat="server"  style="background-color:white;"> 

       <div id="sanjiliandong" style="margin:10px;margin-left:0px;float: left;width: 90%;">
            <!--在这里使用三级联动插件-->
        </div>
        <%if (this.ActionContext.User.UserCode == "Administrator" || this.ActionContext.User.UserCode == "wangyw@dongzhengafc.com"
                || this.ActionContext.User.UserCode =="niwy@dongzhengafc.com")
            {%>
         <div id="Div1" style="margin:10px;margin-left:0px;float:left;" runat="server">
            <!--权限下载控制-->
             <input type="button" onclick="dowloadExcel()" value="下载" style="float:right; margin-right: 20px;"/>
        </div>
         <%}%>
        <div style="height: auto;  overflow-y: scroll;width: 100%;">
                <table class="table" >
                    <thead>
                        <tr  style="border: 1px solid #ccc;">
                            <td id="" class="rowSerialNo">序号</td>
                            <td id="Td1">
                                <label id="Label26" style="">省份</label>
                            </td>
                            <td id="Td2">
                                <label id="Label109" style="">市</label>
                            </td>
                            <td id="Td3">
                                <label id="Label110" style="">县</label>
                            </td>
                              <td id="Td4">
                                <label id="Label1" style="">操作</label>
                            </td>
                        </tr>
                    </thead>
                    <tbody id="shxTable" style="text-align: center;"></tbody>
                </table>
            </div>
    </form>
</body>
</html>
