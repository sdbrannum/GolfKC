/** @type {import('tailwindcss').Config} */
const colors = require('tailwindcss/colors')

export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,vue}",
    "./node_modules/flowbite/**/*.js"
  ],
  theme: {
    colors: {
      brand: {
        100: '#6E5774',
        200: '#5B5271',
        300: '#484564',
        400: '#2A2B47',
        500: '#242039',
        600: '#231C35',
        700: '#120F1C'
      },
      white: colors.white,
      neutral: colors.neutral,
      rose: colors.rose
    },
    extend: {},
  },
  plugins: [
    require('flowbite/plugin')
  ],
}
