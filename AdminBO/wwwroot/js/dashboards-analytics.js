/**
 * Dashboard Analytics
 */

'use strict';

(function () {
  let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

  cardColor = config.colors.cardColor;
  headingColor = config.colors.headingColor;
  legendColor = config.colors.bodyColor;
  labelColor = config.colors.textMuted;
  borderColor = config.colors.borderColor;

  const ReservationData = [
    { month: 'Jan', confirmed: 18, cancelled: -13 },
    { month: 'Feb', confirmed: 7, cancelled: -18 },
    { month: 'Mar', confirmed: 15, cancelled: -9 },
    { month: 'Apr', confirmed: 2, cancelled: -14 },
    { month: 'May', confirmed: 1, cancelled: -5 },
    { month: 'Jun', confirmed: 1, cancelled: -17 },
    { month: 'Jul', confirmed: 9, cancelled: -15 }
  ];

  // Extraire les mois dynamiquement à partir des données
  const months = ReservationData.map(data => data.month);
  const confirmedData = ReservationData.map(data => data.confirmed);
  const cancelledData = ReservationData.map(data => data.cancelled);

  // const totalRevenueChartEl = document.querySelector('#totalRevenueChart');
  // const totalRevenueChartOptions = {
  //   series: [
  //     {
  //       name: "Réservations confirmé",  // Nom de la série pour l'année précédente
  //       data: confirmedData  // Utiliser les réservations confirmées pour chaque mois
  //     },
  //     {
  //       name: "Réservations annulé",  // Nom de la série pour l'année avant l'année précédente
  //       data: cancelledData  // Utiliser les réservations annulées pour chaque mois
  //     }
  //   ],
  //   chart: {
  //     height: 317,
  //     stacked: true,
  //     type: 'bar',
  //     toolbar: { show: false }
  //   },
  //   plotOptions: {
  //     bar: {
  //       horizontal: false,
  //       columnWidth: '30%',
  //       borderRadius: 8,
  //       startingShape: 'rounded',
  //       endingShape: 'rounded'
  //     }
  //   },
  //   colors: [config.colors.primary, config.colors.info],
  //   dataLabels: {
  //     enabled: false
  //   },
  //   stroke: {
  //     curve: 'smooth',
  //     width: 6,
  //     lineCap: 'round',
  //     colors: [cardColor]
  //   },
  //   legend: {
  //     show: true,
  //     horizontalAlign: 'left',
  //     position: 'top',
  //     markers: {
  //       height: 8,
  //       width: 8,
  //       radius: 12,
  //       offsetX: -5
  //     },
  //     fontSize: '13px',
  //     fontFamily: 'Public Sans',
  //     fontWeight: 400,
  //     labels: {
  //       colors: legendColor,
  //       useSeriesColors: false
  //     },
  //     itemMargin: {
  //       horizontal: 10
  //     }
  //   },
  //   grid: {
  //     strokeDashArray: 7,
  //     borderColor: borderColor,
  //     padding: {
  //       top: 0,
  //       bottom: -8,
  //       left: 20,
  //       right: 20
  //     }
  //   },
  //   fill: {
  //     opacity: [1, 1]
  //   },
  //   xaxis: {
  //     categories: months,  // Utilisation des mois dynamiquement extraits
  //     labels: {
  //       style: {
  //         fontSize: '13px',
  //         fontFamily: 'Public Sans',
  //         colors: labelColor
  //       }
  //     },
  //     axisTicks: {
  //       show: false
  //     },
  //     axisBorder: {
  //       show: false
  //     }
  //   },
  //   yaxis: {
  //     labels: {
  //       style: {
  //         fontSize: '13px',
  //         fontFamily: 'Public Sans',
  //         colors: labelColor
  //       }
  //     }
  //   },
  //   responsive: [
  //     {
  //       breakpoint: 1700,
  //       options: {
  //         plotOptions: {
  //           bar: {
  //             borderRadius: 10,
  //             columnWidth: '35%'
  //           }
  //         }
  //       }
  //     },
  //   ],
  //   states: {
  //     hover: {
  //       filter: {
  //         type: 'none'
  //       }
  //     },
  //     active: {
  //       filter: {
  //         type: 'none'
  //       }
  //     }
  //   }
  // };

  // if (typeof totalRevenueChartEl !== undefined && totalRevenueChartEl !== null) {
  //   const totalRevenueChart = new ApexCharts(totalRevenueChartEl, totalRevenueChartOptions);
  //   totalRevenueChart.render();
  // }


  // // Growth Chart - Radial Bar Chart
  // // --------------------------------------------------------------------
  // const growthChartEl = document.querySelector('#growthChart'),
  //   growthChartOptions = {
  //     series: [78],
  //     labels: ['Growth'],
  //     chart: {
  //       height: 240,
  //       type: 'radialBar'
  //     },
  //     plotOptions: {
  //       radialBar: {
  //         size: 150,
  //         offsetY: 10,
  //         startAngle: -150,
  //         endAngle: 150,
  //         hollow: {
  //           size: '55%'
  //         },
  //         track: {
  //           background: cardColor,
  //           strokeWidth: '100%'
  //         },
  //         dataLabels: {
  //           name: {
  //             offsetY: 15,
  //             color: legendColor,
  //             fontSize: '15px',
  //             fontWeight: '500',
  //             fontFamily: 'Public Sans'
  //           },
  //           value: {
  //             offsetY: -25,
  //             color: headingColor,
  //             fontSize: '22px',
  //             fontWeight: '500',
  //             fontFamily: 'Public Sans'
  //           }
  //         }
  //       }
  //     },
  //     colors: [config.colors.primary],
  //     fill: {
  //       type: 'gradient',
  //       gradient: {
  //         shade: 'dark',
  //         shadeIntensity: 0.5,
  //         gradientToColors: [config.colors.primary],
  //         inverseColors: true,
  //         opacityFrom: 1,
  //         opacityTo: 0.6,
  //         stops: [30, 70, 100]
  //       }
  //     },
  //     stroke: {
  //       dashArray: 5
  //     },
  //     grid: {
  //       padding: {
  //         top: -35,
  //         bottom: -10
  //       }
  //     },
  //     states: {
  //       hover: {
  //         filter: {
  //           type: 'none'
  //         }
  //       },
  //       active: {
  //         filter: {
  //           type: 'none'
  //         }
  //       }
  //     }
  //   };
  // if (typeof growthChartEl !== undefined && growthChartEl !== null) {
  //   const growthChart = new ApexCharts(growthChartEl, growthChartOptions);
  //   growthChart.render();
  // }

  // // Order Statistics Chart
  // // --------------------------------------------------------------------
  // const chartOrderStatistics = document.querySelector('#orderStatisticsChart'),
  //   orderChartConfig = {
  //     chart: {
  //       height: 145,
  //       width: 110,
  //       type: 'donut'
  //     },
  //     labels: ['Electronic', 'Sports', 'Decor', 'Fashion'],
  //     series: [50, 85, 25, 40],
  //     colors: [config.colors.success, config.colors.primary, config.colors.secondary, config.colors.info],
  //     stroke: {
  //       width: 5,
  //       colors: [cardColor]
  //     },
  //     dataLabels: {
  //       enabled: false,
  //       formatter: function (val, opt) {
  //         return parseInt(val) + '%';
  //       }
  //     },
  //     legend: {
  //       show: false
  //     },
  //     grid: {
  //       padding: {
  //         top: 0,
  //         bottom: 0,
  //         right: 15
  //       }
  //     },
  //     states: {
  //       hover: {
  //         filter: { type: 'none' }
  //       },
  //       active: {
  //         filter: { type: 'none' }
  //       }
  //     },
  //     plotOptions: {
  //       pie: {
  //         donut: {
  //           size: '75%',
  //           labels: {
  //             show: true,
  //             value: {
  //               fontSize: '18px',
  //               fontFamily: 'Public Sans',
  //               fontWeight: 500,
  //               color: headingColor,
  //               offsetY: -17,
  //               formatter: function (val) {
  //                 return parseInt(val) + '%';
  //               }
  //             },
  //             name: {
  //               offsetY: 17,
  //               fontFamily: 'Public Sans'
  //             },
  //             total: {
  //               show: true,
  //               fontSize: '13px',
  //               color: legendColor,
  //               label: 'Weekly',
  //               formatter: function (w) {
  //                 return '38%';
  //               }
  //             }
  //           }
  //         }
  //       }
  //     }
  //   };
  // if (typeof chartOrderStatistics !== undefined && chartOrderStatistics !== null) {
  //   const statisticsChart = new ApexCharts(chartOrderStatistics, orderChartConfig);
  //   statisticsChart.render();
  // }
});