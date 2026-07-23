<template>
    <div class="food-log-panel">
        <h3 class="panel-title">Recent Days</h3>
        <div v-if="loading" class="panel-loading">Loading...</div>
        <div v-else-if="logs.length === 0" class="panel-empty">No food logs found.</div>
        <ul v-else class="timeline">
            <li v-for="(log, index) in logs" :key="log.date" class="timeline-entry">
                <div class="card">
                    <h3>{{ log.date }}</h3>
                    <p>Calories: {{ log.totalCalories.toFixed(1) }}</p>
                    <p>Carbs: {{ log.totalCarbs.toFixed(1) }}g</p>
                    <p>Protein: {{ log.totalProtein.toFixed(1) }}g</p>
                    <p>Fat: {{ log.totalFat.toFixed(1) }}g</p>
                </div>
                <div v-if="index !== logs.length - 1" class="arrow-down"></div>
            </li>
        </ul>
    </div>
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

            const currentUser = api.getCurrentUser();
            const userId = currentUser?.id || '00000000-0000-0000-0000-000000000001';

            fetch(`/api/FoodLog/user/${userId}`)
                .then(response => {
                    if (!response.ok) throw new Error('Network response was not ok');
                    return response.json();
                })
                .then(json => {
                    const items = json.data || json;

                    // Helper: parse a stored UTC datetime (Z may be missing) → local Date
                    const toLocalDate = (raw) => {
                        if (!raw) return null;
                        const hasTimezone = /[Zz]$|[+-]\d{2}:?\d{2}$/.test(raw);
                        return new Date(hasTimezone ? raw : raw + 'Z');
                    };

                    // --- Aggregate all submissions for the same calendar day ---
                    const byDay = {};
                    items.forEach(log => {
                        const d = toLocalDate(log.dateTime || log.createTime);
                        if (!d || isNaN(d.getTime())) return;
                        const dateKey = d.toLocaleDateString();
                        if (!byDay[dateKey]) {
                            byDay[dateKey] = {
                                date: dateKey,
                                _raw: d,
                                totalCalories: 0,
                                totalCarbs: 0,
                                totalProtein: 0,
                                totalFat: 0
                            };
                        }
                        byDay[dateKey].totalCalories += log.totalCalories || 0;
                        byDay[dateKey].totalCarbs    += log.totalCarbs    || 0;
                        byDay[dateKey].totalProtein  += log.totalProtein  || 0;
                        byDay[dateKey].totalFat      += log.totalFat      || 0;
                    });

                    // Sort newest first, keep 5 most recent days
                    this.logs = Object.values(byDay)
                        .sort((a, b) => b._raw - a._raw)
                        .slice(0, 5);

                    this.loading = false;
                })
                .catch(error => {
                    console.error('FoodLog fetch error:', error);
                    this.loading = false;
                });
        }
    }
};
</script>

<style>
    .food-log-panel {
        display: flex;
        flex-direction: column;
        height: 100%;
    }

    .panel-title {
        color: #2c3e50;
        font-size: 1.1rem;
        font-weight: 600;
        margin-bottom: 1rem;
    }

    .panel-loading,
    .panel-empty {
        color: #6c757d;
        font-size: 0.95rem;
        padding: 1rem 0;
    }

    .timeline {
        list-style-type: none;
        padding: 0;
        margin: 0;
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .timeline-entry {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .card {
        background-color: #fff;
        border: 1px solid #ccc;
        border-radius: 8px;
        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        padding: 16px 20px;
        width: 100%;
        max-width: 300px;
        transition: transform 0.3s ease-in-out;
    }

    .card:hover {
        transform: translateY(-5px);
        box-shadow: 0 6px 16px rgba(0,0,0,0.15);
    }

    .card h3 {
        color: #333;
        font-size: 1rem;
        font-weight: 600;
        margin-bottom: 8px;
    }

    .card p {
        color: #666;
        font-size: 0.9rem;
        margin: 3px 0;
    }

    .arrow-down {
        width: 2px;
        height: 24px;
        background-color: #ccc;
        position: relative;
        bottom: -10px;
    }

    .arrow-down::after {
        content: '';
        width: 0;
        height: 0;
        border-left: 8px solid transparent;
        border-right: 8px solid transparent;
        border-top: 8px solid #ccc;
        position: absolute;
        top: 100%;
        left: -7px;
    }
</style>
