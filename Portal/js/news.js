
/*---����tab��ʽ--*/
$(function(){		
	//�л�
    $('.title-list li').mouseover(function () {
        var liindex = $('.title-list li').index(this);
        if ($(this).children("div:first-child").hasClass("icon-my-06")) {
            //$(this).click(function () {
            //    alert("ok");
            //})
            return;
        } 
		$(this).addClass('on').siblings().removeClass('on');
		$('.list div.news-list').eq(liindex).fadeIn(150).siblings('div.news-list').hide();
		var liWidth = $('.title-list li').width();
		$('.tab .title-list p').stop(false,true).animate({'left' : liindex * liWidth + 'px'},300);
	});
	
	});
	
/*---�쵼����tab��ʽ--*/
$(function(){		
	//�л�
	$('.speech li').mouseover(function(){
		var liindex = $('.speech li').index(this);
		$(this).addClass('on-1').siblings().removeClass('on-1');
		$('.list-1 div.speech-list').eq(liindex).fadeIn(150).siblings('div.speech-list').hide();
		var liWidth = $('.speech li').width();
		$('.tab-1 .speech p').stop(false,true).animate({'left' : liindex * liWidth + 'px'},300);
	});
	
	});
