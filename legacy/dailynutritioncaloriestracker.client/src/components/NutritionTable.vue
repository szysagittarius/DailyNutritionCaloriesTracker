<template>

    <p>This component demonstrates fetching data from the server.</p>
    <div v-if="loading" class="loading">
        Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationvue">https://aka.ms/jspsintegrationvue</a> for more details.
    </div>
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Measurement</th>
                <th>Carbohydrates (g)</th>
                <th>Fat (g)</th>
                <th>Proteins (g)</th>
                <th>Calories (kcal/100g)</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="food in posts" :key="food.name">
                <td>{{ food.name }}</td>
                <td>{{ food.measurement }}</td>
                <td>{{ food.carbs }}</td>
                <td>{{ food.fat }}</td>
                <td>{{ food.protein }}</td>
                <td>{{ food.calories }}</td>
            </tr>
        </tbody>
    </table>
</template>

<script>export default {
  props: {
    posts: Array,
    loading: Boolean
  }
}</script>
