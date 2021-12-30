let livechatbtn = document.querySelector("#livechat-btn");
let livechatbody = document.querySelector("#livechat");
let exitchat = document.querySelector("#exitchat");

livechatbtn.addEventListener('click', function () {

    livechatbody.classList.toggle("d-none");
});

exitchat.addEventListener('click', function () {
    livechatbody.classList.add("d-none");
});

