function setTitle() {
    var origDiv = document.getElementsByClassName("heading")
    var origDivHeight = origDiv[0].clientHeight;
    origDivHeight = (origDivHeight / 100) * 30
    var title = document.getElementById("title")
    title.style.fontSize = origDivHeight + "px"
}


//window.onload = setTitle
//window.addEventListener("resize", setTitle)
//window.onresize = setTitle
