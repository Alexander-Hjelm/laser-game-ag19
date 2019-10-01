function loadModal(itemUrl){
  console.log(itemUrl)

  var modal = document.getElementById("Modal_"+itemUrl);
  var img = document.getElementById("Image_"+itemUrl);
  var modalImg = document.getElementById("Modal_image_"+itemUrl);

  modal.style.display = "block";
  modalImg.src = img.src;

  var span = document.getElementById("Span_"+itemUrl);

  // When the user clicks on <span> (x), close the modal
  span.onclick = function() {
    modal.style.display = "none";
  };

}
