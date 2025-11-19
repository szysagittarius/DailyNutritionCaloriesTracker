<template>
    <svg width="220" height="220" viewBox="-10 -10 220 220">
        <circle cx="100"
                cy="100"
                r="80"
                fill="none"
                stroke="lightgrey"
                stroke-width="20" />
        <circle cx="100"
                cy="100"
                r="80"
                fill="none"
                :stroke="totalCalories > suggestedCalories ? 'red' : 'green'"
                stroke-width="20"
                stroke-linecap="round"
                :stroke-dasharray="circumference"
                :stroke-dashoffset="circumference - (circumference * progressPercentage / 100)"
                transform="rotate(-90 100 100)" />
        <text x="100" y="105"
              font-size="30"
              fill="black"
              text-anchor="middle">
            {{ Math.min(progressPercentage, 100).toFixed(0) }}%
        </text>
    </svg>
</template>

<script>export default {
  props: {
    totalCalories: Number,
    suggestedCalories: Number,
    progressPercentage: Number
  },
  computed: {
    circumference() {
      return 2 * Math.PI * 80; // Calculate the circumference of the circle
    }
  }
}</script>
