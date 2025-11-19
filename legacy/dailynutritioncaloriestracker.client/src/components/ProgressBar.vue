<template>
    <div class="progress-container">
        <!--<div class="progress-bar"
         :style="{width: progressPercent + '%' ,backgroundColor: value> max ? 'red' : color, }"
        >
    </div>-->

        <div class="label">{{ label }}</div>
        <div class="progress-bar"
             :style="progressBarStyle">
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            label: String,
            value: Number,
            max: Number,
            color: String,
        },
        computed: {
            progressPercent() {
                return (this.value / this.max) * 100;
            },
            progressBarStyle() {
                const style = {
                    width: `${Math.min(this.value / this.max * 100, 100)}%`,
                    backgroundColor: this.value > this.max ? 'red' : this.color,
                    height: '100%',
                    borderRadius: '8px',
                    transition: 'width 0.3s ease',
                };
                console.log(style); // Debugging
                return style;
            },
        },
    };
</script>

<style>
    .progress-container {
        width: 100%;
        position: relative; /* Added this line */
        align-items: center;
        background-color: #eee;
        border-radius: 8px;
        height: 20px;
        margin: 10px 0;
    }
    .label {
        color: white; /* Or any color that contrasts well with your progress bar */
        text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.7);
        position: absolute; /* Changed to absolute */

        top: 0;
        left: 0;
        width: 100%; /* Ensure it spans the entire width */
        text-align: center; /* Center the text horizontally */
        z-index: 1; /* Ensure it's above the progress bar */
    }

    .progress-bar {
        height: 100%;
        border-radius: 8px;
        transition: width 0.3s ease;
        position: relative;
    }
</style>
