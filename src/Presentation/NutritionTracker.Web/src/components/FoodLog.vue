<template>
    <ul class="timeline">
        <li v-for="(log, index) in logs" :key="log.id || log.dateTime" class="timeline-entry">
            <div class="card">
                <h3>{{ formatDate(log.dateTime) }}</h3>
                <p>Calories: {{ log.totalCalories }}</p>
                <p>Carbs: {{ log.totalCarbs }}</p>
                <p>Protein: {{ log.totalProtein }}</p>
                <p>Fat: {{ log.totalFat }}</p>
            </div>
            <!-- Arrow element, not added for the last item -->
            <div v-if="index !== logs.length - 1" class="arrow-down"></div>
        </li>
    </ul>
</template>

<script>
import api from '../services/api'

export default {
    name: 'FoodLog',
    data() {
        return {
            logs: [],
            loading: false
        };
    },
    mounted() {
        this.fetchData();
    },
    methods: {
        fetchData() {
            this.loading = true;
            
            // Get current user (same logic as MyFoodLog.vue)
            const currentUser = api.getCurrentUser();
            const userId = currentUser?.id || '00000000-0000-0000-0000-000000000001';
            
            console.log('FoodLog: Fetching for userId:', userId); // Debug log
            
            fetch(`foodlog/GetUserFoodLogs/${userId}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(json => {
                    console.log('FoodLog: Raw API response:', json); // Debug log
                    console.log('FoodLog: Number of items received:', json.length); // Debug log
                    
                    // Sort by date (newest first) and take only top 5
                    const sortedLogs = json.sort((a, b) => 
                        new Date(b.dateTime || b.createTime) - new Date(a.dateTime || a.createTime)
                    );
                    
                    console.log('FoodLog: After sorting:', sortedLogs); // Debug log
                    
                    this.logs = sortedLogs.slice(0, 5); // Take only top 5 results
                    
                    console.log('FoodLog: Final logs to display:', this.logs); // Debug log
                    console.log('FoodLog: Number of logs to display:', this.logs.length); // Debug log
                    
                    this.loading = false;
                })
                .catch(error => {
                    console.error('There was a problem with the fetch operation:', error);
                    this.loading = false;
                });
        },
        formatDate(dateString) {
            if (!dateString) return 'N/A';
            const date = new Date(dateString);
            return date.toLocaleDateString();
        }
    }
};
</script>

<style>
    .timeline {
        list-style-type: none;
        padding: 0;
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .timeline-entry {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .card {
        background-color: #fff;
        border: 1px solid #ccc;
        border-radius: 8px;
        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        padding: 20px;
        width: 300px; /* Adjust width as necessary */
        transition: transform 0.3s ease-in-out;
    }

        .card:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 16px rgba(0,0,0,0.15);
        }

    .arrow-down {
        width: 2px; /* This makes it look like a line */
        height: 30px; /* Adjust height for spacing between cards */
        background-color: #ccc; /* Line color */
        position: relative;
        bottom: -10px;
    }

        .arrow-down::after {
            content: '';
            width: 0;
            height: 0;
            border-left: 10px solid transparent;
            border-right: 10px solid transparent;
            border-top: 10px solid #ccc; /* Arrow color */
            position: absolute;
            top: 100%;
            left: -9px; /* Centers the arrow */
        }

    h3 {
        color: #333;
        font-size: 18px;
        margin-bottom: 10px;
    }

    p {
        color: #666;
        font-size: 16px;
        margin: 5px 0;
    }
</style>
