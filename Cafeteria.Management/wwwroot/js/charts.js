Chart.register(ChartDataLabels);

window.chartManager = {
    _charts: {},

    renderBarChart: function (canvasId, labels, data, foodNames) {
        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
        }

        const canvas = document.getElementById(canvasId);
        if (!canvas) return;

        const combinedLabels = labels.map((date, i) => [date, foodNames[i]]);

        this._charts[canvasId] = new Chart(canvas.getContext('2d'), {
            type: 'bar',
            data: {
                labels: combinedLabels,
                datasets: [{
                    data: data,
                    backgroundColor: 'rgba(30, 54, 108, 0.75)',
                    borderColor: 'rgba(30, 54, 108, 1)',
                    borderWidth: 1,
                    borderRadius: 5,
                    borderSkipped: false,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                layout: {
                    padding: { top: 24 }
                },
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            title: (items) => labels[items[0].dataIndex],
                            label: (item) => '  ' + foodNames[item.dataIndex],
                            afterLabel: (item) => '  Sold: ' + item.raw + ' time' + (item.raw !== 1 ? 's' : ''),
                        }
                    },
                    datalabels: {
                        anchor: 'end',
                        align: 'end',
                        color: '#1e366c',
                        font: { weight: 'bold', size: 12 },
                        formatter: (value) => value,
                    }
                },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: {
                            font: (ctx) => ({
                                size: 11,
                                style: ctx.tick?.label && ctx.tick.label === ctx.tick.label[1] ? 'italic' : 'normal',
                            }),
                            maxRotation: 30,
                            minRotation: 0,
                        }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1, precision: 0 },
                        grid: { color: 'rgba(0,0,0,0.05)' },
                        title: {
                            display: true,
                            text: 'Times Sold',
                            color: '#6c757d',
                            font: { size: 12 },
                        }
                    }
                }
            }
        });
    },

    destroy: function (canvasId) {
        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
            delete this._charts[canvasId];
        }
    }
};
