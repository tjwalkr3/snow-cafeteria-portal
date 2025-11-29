window.sidebarManager = {
  initialized: false,

  init: function () {
    const checkbox = document.getElementById("sidebarExpander");
    if (!checkbox) return;

    if (!this.initialized) {
      const savedState = localStorage.getItem("sidebarExpanded");
      if (savedState === "true") {
        checkbox.checked = true;
      }

      checkbox.addEventListener("change", this.saveState);
      this.initialized = true;
    } else {
      const savedState = localStorage.getItem("sidebarExpanded");
      if (savedState === "true") {
        checkbox.checked = true;
      }
    }
  },

  saveState: function () {
    localStorage.setItem("sidebarExpanded", this.checked);
  },
};

(function () {
  const checkbox = document.getElementById("sidebarExpander");
  if (checkbox) {
    const savedState = localStorage.getItem("sidebarExpanded");
    if (savedState === "true") {
      checkbox.checked = true;
    }
  }
})();

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", function () {
    window.sidebarManager.init();
  });
} else {
  window.sidebarManager.init();
}

if (window.Blazor) {
  window.Blazor.addEventListener("enhancedload", function () {
    window.sidebarManager.init();
  });
}
