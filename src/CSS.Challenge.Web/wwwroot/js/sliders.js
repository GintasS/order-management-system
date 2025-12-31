function bindSliderToLabel(sliderId, labelId) {
    const slider = document.getElementById(sliderId);
    const label = document.getElementById(labelId);

    label.textContent = slider.value;

    slider.addEventListener("input", () => {
        label.textContent = slider.value;
    });
}

bindSliderToLabel("heaterCapSlider", "heaterCapLabel");
bindSliderToLabel("freezerCapSlider", "freezerCapLabel");
bindSliderToLabel("shelfCapSlider", "shelfCapLabel");
bindSliderToLabel("freshnessMinSlider", "freshnessMinLabel");
bindSliderToLabel("freshnessMaxSlider", "freshnessMaxLabel");
bindSliderToLabel("pickupTimeMinSlider", "pickupTimeMinLabel");
bindSliderToLabel("pickupTimeMaxSlider", "pickupTimeMaxLabel");
bindSliderToLabel("orderCountSlider", "orderCountLabel");
